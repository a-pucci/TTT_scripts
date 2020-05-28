using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallPhysics:MonoBehaviourPun, IPunObservable, IModdable {

	[Tooltip("Ball direction and speed")]
	public Vector3 velocity;
	[Tooltip("Current Damage on Hit")]
	public int currentDamage;
	[Tooltip("Can damage")]
	public bool canDamage;
	[Tooltip("This Ball's Type")]
	public BallType ballType;
	public LayerMask collisionMask;
	[Tooltip("Draw sphere or wire sphere")]
	public BallType.DrawType gizmoDrawType;
	[HideInInspector]
	public SwingPower ballSwingPower;
	[HideInInspector] public Status status;

	#region Other Variables

	private List<Vector3> collisionPoints;
	[HideInInspector]
	public int currentBounces;
	public bool stopped;
	public MeshRenderer meshRenderer;
	private bool canSwing = true;
	private bool canTrap = false;
	private TrailRenderer trailRenderer;
	private Vector3 initialSize;

	private Vector3 networkPosition;
	//private float predictionTime;
	private Vector3 pastPosition;
	private float serializationRate;

	#endregion

	#region MonoBehaviour Functions

	private void Awake() {
		collisionPoints = new List<Vector3>();
		currentBounces = 0;
		trailRenderer = gameObject.GetComponent<TrailRenderer>();
		trailRenderer.enabled = false;
		initialSize = transform.localScale;
		meshRenderer = GetComponentInChildren<MeshRenderer>();
	}

	//Checking racket collision. I wish i could have handled everything in my own personal coroutine, but somethings just don't work out
	private void OnTriggerEnter(Collider other) {
		if ((PhotonNetwork.IsConnected && photonView.IsMine) || !PhotonNetwork.IsConnected) {
			var surface = other.gameObject.GetComponent<Surface>();
			if (surface != null) {
				surface.TriggerHit(this);
			}
		}
	}

	//The coroutine which checks for collisions and moves the ball
	private void FixedUpdate() {
		//if (PhotonNetwork.IsConnected && photonView.IsMine) {
		//	photonView.RPC(nameof(Move), RpcTarget.All);
		//}
		//else
		if (transform.position.y <= -30) {
			GameManager.Instance.ballSpawner.SpawnBall(PlayerNumber.Top);
		}
		if (!PhotonNetwork.IsConnected || PhotonNetwork.IsConnected && photonView.IsMine) {
			Move();
		}
		else if (PhotonNetwork.IsConnected && !photonView.IsMine) {
			transform.position = Vector3.Lerp(transform.position, networkPosition, serializationRate);
			//predictionTime += 1000 / PhotonNetwork.SerializationRate;
			//transform.position = Vector3.Lerp(transform.position, networkPosition, 0.3f);
		}
		//}
	}

	[PunRPC]
	private void Move() {
		if (!GameManager.Instance.isPaused && !stopped) {
			RaycastHit hitInfo;

			//Placing the trail renderer behind the ball
			trailRenderer.sortingOrder = (int)-transform.position.z - 1;

			//Applying gravity
			velocity += Vector3.down * ballType.gravity;

			//Checking if from my current position, towards my speed, for my speed's magnitude + sphere radius there's a collision
			//Wall collision
			Debug.DrawRay(transform.position, velocity.normalized * ((velocity.magnitude * Time.deltaTime) + ballType.radius), Color.red, 1000f, true);
			//((photonView.IsMine && PhotonNetwork.IsConnected) || !PhotonNetwork.IsConnected) && 
			if (Physics.Raycast(transform.position, velocity, out hitInfo, (velocity.magnitude * Time.deltaTime) + ballType.radius, collisionMask, QueryTriggerInteraction.Ignore)) {
				collisionPoints.Add(hitInfo.point);
				var surface = hitInfo.transform.gameObject.GetComponent<Surface>();
				if (surface) {
					surface.RaycastHit(this, hitInfo);
				}
			}
			else {
				transform.Translate(velocity * Time.deltaTime);
			}
		}
	}

	public void ToggleRenderer(bool enable) {
		if (PhotonNetwork.IsConnected) {
			photonView.RPC(nameof(OnlineToggleRenderer), RpcTarget.All, enable);
		}
		else {
			meshRenderer.enabled = enable;
		}
	}

	[PunRPC]
	private void OnlineToggleRenderer(bool enable) {
		meshRenderer.enabled = enable;
	}
	
	[PunRPC]
	public void PlayHitEffect(string characterCharge, Vector3 position) {
		ballType.swingEffects.Get(characterCharge)?.Play(position, Vector3.zero);
		ballType.swingEffects.Get("All")?.Play(position, Vector3.zero);
	}
	
	#endregion

	#region Other Functions

	//Function used to handle what happens when the ball is hit by a racket
	
	// public Vector3 Strike(Vector3 newDirection,SwingType swingType) {
	// 	GameObject newInstance;
	// 	VisualEffect vfx;
	//
	// 	currentBounces = 0;
	//
	// 	#region Swing VFX
	// 	switch (swingType.strengthMode) {
	// 		case SwingPower.Regular:
	// 			AudioManager.instance.PlaySound(ballType.ballHitSound,false,0.8f);
	// 			vfx = ballType.mediumShotEffect;
	// 			break;
	// 		case SwingPower.Strong:
	// 			AudioManager.instance.PlaySound(ballType.ballHitSoundHard,false, 0.8f);
	// 			vfx = ballType.strongShotEffect;
	// 			break;
	// 		default:
	// 			AudioManager.instance.PlaySound(ballType.ballHitSound,false);
	// 			vfx = ballType.weakShotEffect;
	// 			break;
	// 	}
	// 	// if (vfx != null) {
	// 	// 	ballType.effectHandler.visualEffect = vfx;
	// 	// 	newInstance = Instantiate(ballType.effectHandler.gameObject,vfx.localPosition,Quaternion.identity);
	// 	// 	newInstance.transform.position += transform.position;
	// 	// }
	// 	#endregion
	// 	//Sets a timer for which the ball can not be hit again
	// 	StartCoroutine(WaitSwing());
	// 	//Returns a new destination for the ball based on the calculated swing
	// 	return transform.position + (newDirection * swingType.swingStrength);
	//}

	//Disables swinging, waits for ballType.swingTimer then enables swinging again
	private IEnumerator WaitSwing() {
		canSwing = false;
		yield return new WaitForSecondsRealtime(ballType.swingTimer);
		canSwing = true;
	}

	#endregion

	#region Gizmo Functions
	//Draws the ball's collision sphere and the ball's collision points
	private void OnDrawGizmos() {
		Gizmos.color = ballType.gizmoColor;
		switch (gizmoDrawType) {
			case BallType.DrawType.Sphere:
				Gizmos.DrawSphere(transform.position,ballType.radius);
				break;
			case BallType.DrawType.Wire:
				Gizmos.DrawWireSphere(transform.position,ballType.radius);
				break;
		}
		if (ballType.gizmoDrawCollision && collisionPoints != null) {
			for (int i = 0; i < collisionPoints.Count; i++) {
				Gizmos.DrawIcon(collisionPoints[i],ballType.gizmoDrawCollisionImage.name);
			}
		}
	}

	#endregion
	
	#region Status Functions

	public void SetSpeed(float percentage) {
		// Not implemented
	}

	public void ResetSpeed() {
		// Not implemented
	}

	public void SetDash(float percentage) {
		// Not implemented
	}

	public void ResetDash() {
		// Not implemented
	}

	public void SetSize(float percentage) {
		transform.localScale = initialSize + ((initialSize / 100) * percentage);
	}

	public void ResetSize() {
		transform.localScale = initialSize;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.IsWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(velocity);
		}
		else {
			networkPosition = (Vector3)stream.ReceiveNext();
			velocity = (Vector3)stream.ReceiveNext();

			float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
			pastPosition = transform.position;
			//predictionTime = 0f;
			networkPosition += (velocity * lag);
			serializationRate = Vector3.Distance(transform.position, networkPosition) * (1f / PhotonNetwork.SerializationRate);
		}
	}

	//public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
	//	if (stream.IsWriting) {
	//		//stream.SendNext(velocity);
	//		stream.SendNext(transform.position);
	//	}
	//	else {
	//		//velocity = (Vector3)stream.ReceiveNext();
	//		transform.position = (Vector3)stream.ReceiveNext();
	//	}
	//}

	#endregion
}
