using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Photon.Pun;

#region Enums

public enum PlayerNumber {
	None,
	Bottom,
	Top
}

[Flags]
public enum SwingPower {
	Light = 1 << 1,
	Regular = 2 << 2,
	Strong = 4 << 4,
	All = Light | Regular | Strong
}

public enum SwingOrientation {
	Left,
	Right,
	Forward
}

public enum PlayerStatus {
	None,
	Dashing,
	Swinging,
	Charging,
	Stunned
}

public enum SlowingType {
	Charge,
	Trap
}

#endregion

public class CharacterManager : MonoBehaviourPun, IPunObservable, IModdable {

	#region Public Variables

	//Current Movement Vector
	public Vector3 input;
	public SwingPower currentCharge = SwingPower.Regular;
	public SwingOrientation orientation = SwingOrientation.Left;
	public MovementType movementType;
	public SwingType swingType;
	//public PlayerType playerType;
	public Animator animator;
	public string playerInputSuffix;
	public bool canMove = true;
	public PlayerPosition playerPosition;
	public PlayerStatus playerStatus;
	public Animator auraAnimator;
	[InfoBox("x => minX\ny => minZ\nz => maxX\nw => maxZ")]
	public Vector4 maxMovement;
	[HideInInspector] public ArenaMultipliers arenaMultipliers;
	[HideInInspector] public Color auraColor;
	
	
	#endregion

	#region Private Variables

	private Rigidbody rigidbodyComponent;
	private Collider playerCollider;
	private SpriteRenderer sprite;
	private bool chargeReduce;
	private float initialSpeedWithModifier;
	private float currentSpeed;
	private float currentDashDistance;
	[HideInEditorMode, ReadOnly]
	public float currentSwingStrength;
	private Vector3 currentSwingDirection;
	private Transform feetTransform;
	private ParticleSystem particleSystem;
	private ParticleSystemRenderer particleSystemRenderer;
	private PlayerNumber playerSelect;
	//private float swingCharge;
	private Coroutine chargeCoroutine;
	private float chargedTime;
	private GameObject leftDashSwingCollider;
	private GameObject rightDashSwingCollider;
	private EffectCollection effects;
	private Transform aura;
	private Rewired.Player player;
	private Vector3 networkPosition;
	private float networkDistance;

	#endregion

	#region Unity Callbacks

	private void Start() {

		//Assign variables
		animator = gameObject.GetComponent<Animator>();
		rigidbodyComponent = gameObject.GetComponent<Rigidbody>();
		particleSystem = gameObject.transform.Find("Sprite").GetComponent<ParticleSystem>();
		particleSystemRenderer = gameObject.transform.Find("Sprite").GetComponent<ParticleSystemRenderer>();

		leftDashSwingCollider = transform.Find("LeftDashSwingCollider").gameObject;
		leftDashSwingCollider.SetActive(false);
		rightDashSwingCollider = transform.Find("RightDashSwingCollider").gameObject;
		rightDashSwingCollider.SetActive(false);

		playerCollider = GetComponent<BoxCollider>();
			
		sprite = gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();
		effects = gameObject.GetComponent<Surface>().surfaceType.effectCollection;
		aura = new GameObject("Aura").transform;
		aura.SetParent(transform);
		aura.position = transform.position; 
		aura.localScale = Vector3.one;
		
		initialSpeedWithModifier = movementType.speed * arenaMultipliers.speed;
		currentSpeed = initialSpeedWithModifier;
		currentDashDistance = movementType.dashDistance;

		feetTransform = transform.Find("FeetPosition");

		DOTween.Init();
		DOTween.defaultEaseType = movementType.dashEase;
		playerSelect = GameManager.Instance.net == null ? PlayerNumber.Top : transform.position.z < GameManager.Instance.net.position.z ? PlayerNumber.Bottom : PlayerNumber.Top;
		if (!PhotonNetwork.IsConnected) {
			switch (playerSelect) {
				case PlayerNumber.Bottom:
					playerInputSuffix = "1";
					playerPosition = PlayerPosition.Bottom;
					player = Rewired.ReInput.players.GetPlayer(0);
					break;
				case PlayerNumber.Top:
					playerInputSuffix = "2";
					playerPosition = PlayerPosition.Top;
					player = Rewired.ReInput.players.GetPlayer(1);
					break;
				default:
					playerInputSuffix = "";
					break;
			}
		}
		else {
			if (playerSelect == PlayerNumber.Bottom && PhotonNetwork.IsMasterClient) {
				photonView.RequestOwnership();
				playerPosition = PlayerPosition.Bottom;
				player = Rewired.ReInput.players.GetPlayer(0);
			}
			else if (playerSelect == PlayerNumber.Top && !PhotonNetwork.IsMasterClient) {
				photonView.RequestOwnership();
				playerPosition = PlayerPosition.Top;
				player = Rewired.ReInput.players.GetPlayer(0);
			}
		}
		if (player != null) {
			player.controllers.maps.SetAllMapsEnabled(false);
			player.controllers.maps.SetMapsEnabled(true, Rewired.ControllerType.Joystick, RewiredConsts.Category.Default, RewiredConsts.Layout.Joystick.TypeA);
		}
	}

	// Update is called once per frame
	private void Update () {
		if (canMove && !GameManager.Instance.isPaused && (!PhotonNetwork.IsConnected || (PhotonNetwork.IsConnected && photonView.IsMine))) {
			input = ReadMovement();
			if (PhotonNetwork.IsConnected && player != null) {
				if (player.GetButtonDown(RewiredConsts.Action.Dash) && playerStatus == PlayerStatus.None) {
					photonView.RPC(nameof(Dash), RpcTarget.All);
				}
				else if (player.GetButtonDown(RewiredConsts.Action.Charge) && playerStatus == PlayerStatus.None) {
					AssignChargeCoroutine();
					photonView.RPC(nameof(Move), RpcTarget.All);
				}
				else if ((player.GetButtonDown(RewiredConsts.Action.Swing) && playerStatus == PlayerStatus.None) || (chargeCoroutine != null && (player.GetButtonUp(RewiredConsts.Action.Charge) || !player.GetButton(RewiredConsts.Action.Charge)))) {
					StopChargeCoroutine();
					photonView.RPC(nameof(Swing), RpcTarget.All, orientation, currentCharge);
					//ReleaseCharge(Color.white);
				}
				else {
					//Regular movement
					photonView.RPC(nameof(Move), RpcTarget.All);
				}
			}
			else if (player != null) {
				if (player.GetButtonDown(RewiredConsts.Action.Dash) && playerStatus == PlayerStatus.None) {
					Dash();
				}
				else if (player.GetButtonDown(RewiredConsts.Action.Charge) && playerStatus == PlayerStatus.None) {
					AssignChargeCoroutine();
					Move();
				}
				else if ((player.GetButtonDown(RewiredConsts.Action.Swing) && playerStatus == PlayerStatus.None) || (chargeCoroutine != null && (player.GetButtonUp(RewiredConsts.Action.Charge) || !player.GetButton(RewiredConsts.Action.Charge)))) {
					StopChargeCoroutine();
					Swing(orientation, currentCharge);
					//ReleaseCharge(Color.white);
				}
				else {
					//Regular movement
					Move();
				}
			}
		}
		else if (PhotonNetwork.IsConnected && !photonView.IsMine) {
			transform.position = Vector3.Lerp(transform.position, networkPosition, networkDistance * (1f / PhotonNetwork.SerializationRate));
		}
	}

	#endregion
	
	#region Movement Functions

	//Reads player input (could be moved to different class altoghether)
	private Vector3 ReadMovement() {
		var movementVector = Vector3.zero;
		if (player != null) {
			movementVector = new Vector3(player.GetAxis(RewiredConsts.Action.MoveHorizontal),0,player.GetAxis(RewiredConsts.Action.MoveVertical));
			animator.SetFloat("x", movementVector.x);
			animator.SetFloat("y", movementVector.z);
		}
		return movementVector; 
	}

	//Handles player input
	[PunRPC]
	public void Move() {
		Vector3 newPosition = transform.position + (input * currentSpeed);
		if (input != new Vector3(0,0,0)) {
			animator.SetBool("IsMoving",true);
			rigidbodyComponent.MovePosition(newPosition);
		} else {
			//rigidbodyComponent.velocity = Vector3.zero;
			animator.SetBool("IsMoving",false);
		}
	}
	
	//Function that handles speed reduction, whenever a player is charging a shot or steps on a trap
	public void ReduceSpeed (SlowingType type) {
		switch (type) {
			case SlowingType.Charge:
				if (!chargeReduce) {
					StartCoroutine(ChargeSlowing(swingType.chargingDivider, swingType.animatorChargingDivider));
					chargeReduce = true;
				}
				break;
			// case SlowingType.Trap:
			// 	if (!trapReduce) {
			// 		animator.SetFloat("Speed",animator.GetFloat("Speed") * playerType.animatorTrapDivider);
			// 		currentSpeed /= playerType.trapDivider;
			// 		trapReduce = true;
			// 	}
			// 	break;
		}
		if (currentSpeed > initialSpeedWithModifier) {
			currentSpeed = initialSpeedWithModifier;
		}
	}

	private IEnumerator ChargeSlowing(float chargingDivider, float animatorChargingDivider) {
		float startingSpeed = currentSpeed;
		float animatorStartingSpeed = animator.GetFloat("Speed");
		float animatorSpeed = animatorStartingSpeed;

		while (playerStatus == PlayerStatus.Charging) {
			if (currentCharge == SwingPower.Light) {
				currentSpeed = Mathf.Lerp(currentSpeed, startingSpeed * chargingDivider, chargedTime / swingType.lightSwingChargeTime);
				animatorSpeed = Mathf.Lerp(animatorSpeed, animatorStartingSpeed * animatorChargingDivider, chargedTime / swingType.lightSwingChargeTime);
				animator.SetFloat("Speed", animatorSpeed);
			}
			yield return null;
		}
		
		currentSpeed = startingSpeed;
		animator.SetFloat("Speed", animatorStartingSpeed);
		chargeReduce = false;
	}

	[PunRPC]
	private void StopChargeCoroutine() {
		if (chargeCoroutine != null) {
			StopCoroutine(chargeCoroutine);
			chargeCoroutine = null;
		}
	}

	[PunRPC]
	private void AssignChargeCoroutine() {
		chargeCoroutine = StartCoroutine(StartCharge());
	}
	
	//Function that handles speed reduction, whenever a player stops charging a shot or steps out a trap
	public void RaiseSpeed (SlowingType type) {
		switch (type) {
			case SlowingType.Charge:
				if (chargeReduce) {
					animator.SetFloat("Speed",animator.GetFloat("Speed") / swingType.animatorChargingDivider);
					currentSpeed /= swingType.chargingDivider;
					chargeReduce = false;
				}
				break;
			// case SlowingType.Trap:
			// 	if (trapReduce) {
			// 		animator.SetFloat("Speed",animator.GetFloat("Speed") / playerType.animatorTrapDivider);
			// 		currentSpeed *= playerType.trapDivider;
			// 		trapReduce = false;
			// 	}
			//	break;
		}
		if (currentSpeed < initialSpeedWithModifier) {
			currentSpeed = initialSpeedWithModifier;
		}
	}

	#endregion

	#region Dash Functions

	//Function used to handle the player dash
	[PunRPC]
	public void Dash() {
		GameAnalytics.AddEvent(playerPosition, EventKeys.Dash);
		
		//Movement vector normalized for simplicity
		input.Normalize();
		//Checking if my dash-to position is valid
		input = LandingPosition(input);
		
		if (input != transform.position) {
			playerStatus = PlayerStatus.Dashing;
			animator.SetBool("IsDashing",true);
			ChangeStrength(currentCharge);

			switch (orientation) {
				case SwingOrientation.Right:
					rightDashSwingCollider.SetActive(true);
					break;
				case SwingOrientation.Left:
					leftDashSwingCollider.SetActive(true);
					break;
			}
			
			//Play audio sfx
			movementType.effectCollection.Get("Dash")?.Play(transform.position, Vector3.zero, transform);

			//Setting the current sprite to be rendered as the particle
			particleSystemRenderer.material.mainTexture = sprite.sprite.texture;			
			particleSystem.Play();

			//Translate the player using the DOTween Plugin
			transform.DOMove(input, movementType.dashDuration).onComplete = StopDashing;
			
		}
	}

	//Function that returns the player's landing position based on the intended dash position
	private Vector3 LandingPosition(Vector3 direction) {
		RaycastHit hitInfo;
		
		Debug.DrawRay(transform.position, direction * currentDashDistance, Color.red,100f);
		Ray ray = new Ray(transform.position, direction);
		Vector3 rayDestination = ray.GetPoint(currentDashDistance);
		Vector3 landingPosition;
		if (Physics.Raycast(ray,  out hitInfo, currentDashDistance, LayerMask.GetMask("Wall") | LayerMask.GetMask("Net"))) {
			landingPosition = hitInfo.point + hitInfo.normal * playerCollider.bounds.extents.magnitude/2;
			landingPosition = new Vector3(landingPosition.x, transform.position.y , landingPosition.z);	
		}
		else {
			landingPosition = direction * currentDashDistance + transform.position;	
		}
		return new Vector3(Mathf.Clamp(landingPosition.x, maxMovement.x, maxMovement.z), landingPosition.y, Mathf.Clamp(landingPosition.z, maxMovement.y, maxMovement.w));
	}

	//Function that is called at the end of the player's dash
	private void StopDashing () {
		playerStatus = PlayerStatus.None;
		ChangeStrength(SwingPower.Light);
		leftDashSwingCollider.SetActive(false);
		rightDashSwingCollider.SetActive(false);
		animator.SetBool("IsDashing",false);
	}

	#endregion

	#region Swing Functions

	//Function that handles the swinging of the racket, enables necessary animations and sets swing power
	[PunRPC]
	public void Swing(SwingOrientation orientation, SwingPower power) {
		playerStatus = PlayerStatus.Swinging;
		GameObject ball = GameManager.Instance.ballSpawner.GetCurrentBall();
		if (ball) {
			orientation = ball.transform.position.x >= transform.position.x ? SwingOrientation.Right : SwingOrientation.Left;
		}
		ChangeStrength(power);
		
		switch (orientation) {
			case SwingOrientation.Left:
				animator.SetBool("IsSwingingLeft",true);
				break;
			case SwingOrientation.Right:
				animator.SetBool("IsSwingingRight",true);
				break;
			default:
				throw new ArgumentOutOfRangeException("orientation", orientation, null);
		}
		if(PhotonNetwork.IsConnected) photonView.RPC(nameof(PlaySwingEffect), RpcTarget.All);
		else PlaySwingEffect();
		
	}

	[PunRPC]
	private void PlaySwingEffect() {
		swingType.swingEffects.Get("Swing").Play(transform.position, Vector3.zero);
	}

	//Function called when the player stops swinging, stops all animations
	public void StopSwing() {
		playerStatus = PlayerStatus.None;
		animator.SetBool("IsSwingingLeft",false);
		animator.SetBool("IsSwingingRight",false);
		animator.SetBool("IsSwingingTop",false);
		ReleaseCharge();
	}

	private IEnumerator StartCharge() {
		chargedTime = 0;
		playerStatus = PlayerStatus.Charging;
		ReduceSpeed(SlowingType.Charge);
		// auraAnimator.gameObject.SetActive(true);
		// auraAnimator.SetBool("Charged", false);

		// Light Charge
		if(PhotonNetwork.IsConnected) photonView.RPC(nameof(PlayAuraEffect), RpcTarget.All, "AuraBase");
		else PlayAuraEffect("AuraBase");
		
		while (chargedTime <= swingType.lightSwingChargeTime && player.GetButton(RewiredConsts.Action.Charge) && playerStatus != PlayerStatus.Stunned) {
			//sprite.color = playerType.swingType.lightSwingColor;
			currentCharge = SwingPower.Light;
			chargedTime += Time.deltaTime;
			yield return null;
		}

		// Strong Charge
		if(PhotonNetwork.IsConnected) photonView.RPC(nameof(PlayAuraEffect), RpcTarget.All, "ChargedAura");
		else PlayAuraEffect("ChargedAura");
		
		while (player.GetButton(RewiredConsts.Action.Charge) && playerStatus != PlayerStatus.Stunned) {
			//sprite.color = playerType.swingType.strongSwingColor;
			currentCharge = SwingPower.Strong;
			//auraAnimator.SetBool("Charged", true);
			yield return null;
		}
		if(PhotonNetwork.IsConnected) photonView.RPC(nameof(ReleaseCharge), RpcTarget.All);
		else ReleaseCharge();
		yield return null;
	}

	[PunRPC]
	private void PlayAuraEffect(string auraName) {
		effects.Get(auraName)?.Play(transform.position, Vector3.zero, aura, auraColor);
	}

	//Releases the player's charge, and sets its current power
	[PunRPC]
	public void ReleaseCharge() {
		chargedTime = 0;
		playerStatus = PlayerStatus.None;
		currentCharge = SwingPower.Regular;
		if (aura && aura.childCount > 0) {
			foreach (Transform child in aura.transform) {
				Destroy(child.gameObject);
			}
		}
		// auraAnimator.SetBool("Charged", false);
		// auraAnimator.gameObject.SetActive(false);
	}

	//Changes the strength of the next swing
	public void ChangeStrength(SwingPower newStrength) {
		switch (newStrength) {
			case SwingPower.Light:
				currentSwingStrength = swingType.lightSwing * arenaMultipliers.strength;
				break;
			case SwingPower.Regular:
				currentSwingStrength = swingType.regularSwing * arenaMultipliers.strength;
				break;
			case SwingPower.Strong:
				currentSwingStrength = swingType.strongSwing * arenaMultipliers.strength;
				break;
			default:
				currentSwingStrength = 0;
				break;
		}
	}
	
	public void PlayStepEffect() {
		movementType.effectCollection.Get("Step")?.Play(transform.position, Vector3.zero, transform);
	}

	#endregion
	
	#region Status Functions

	public void SetSpeed(float percentage) {
		currentSpeed = (currentSpeed / 100) * percentage;
	}

	public void ResetSpeed() {
		currentSpeed = initialSpeedWithModifier;
	}

	public void SetDash(float percentage) {
		currentDashDistance = currentDashDistance + ((currentDashDistance / 100) * percentage);
	}

	public void ResetDash() {
		currentDashDistance = movementType.dashDistance;
	}

	public void SetSize(float percentage) {
		// Not implemented
	}

	public void ResetSize() {
		// Not implemented
	}

	//public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
	//	if (stream.IsWriting) {
	//		stream.SendNext(input);
	//		stream.SendNext(transform.position);
	//	}
	//	else {
	//		input = (Vector3)stream.ReceiveNext();
	//		transform.position = (Vector3)stream.ReceiveNext();
	//	}
	//}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.IsWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(input);
		}
		else {
			networkPosition = (Vector3)stream.ReceiveNext();
			input = (Vector3)stream.ReceiveNext();

			float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));

			networkPosition += (input * lag);
			networkDistance = Vector3.Distance(transform.position, networkPosition);
		}
	}

	#endregion
}
