using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using Photon.Pun;

public class BackWall : Surface {

	public MeshRenderer floorRenderer;

	public PlayerPosition playerPosition;

	[InfoBox("This is the color the floor will slowly become while the wall is being hit")]
	public Color damageColor;

	private Color startingColor;

	private Animator animator;
	[ShowInInspector]private bool isActive = true;
	
	[HideInEditorMode, ShowInInspector, ReadOnly]
	private float currentHits;
	private BallPhysics currentBall;

	public event Action<BackWall> OnHit;

	[Range(0f, 100f)]
	[Tooltip("La percentuale di velocità che avrà la palla dopo aver colpito la parete, rispetto alla velocità attuale della palla. Applicato solo su la palla forte")]
	public float strongHitSpeedDecrease;
	
	private void Awake() {
		animator = GetComponent<Animator>();
		if (floorRenderer) {
			startingColor = floorRenderer.material.color;
		}
		currentHits = 0;
	}

	public override void RaycastHit(BallPhysics ball, RaycastHit hitInfo) {
		//Reflect Handling
		
		if (ball) {
			currentBall = ball;
			if (isActive && ball.ballSwingPower == SwingPower.Strong) {
				if (ball.photonView.IsMine) {
					ball.photonView.RequestOwnership();
				}
				ball.velocity = (ball.velocity * strongHitSpeedDecrease) / 100f;
			}
			base.RaycastHit(ball, hitInfo);
		}

		//Swap Handling
		if (isActive) {
			if (surfaceType.effectCollection != null) {
				if(PhotonNetwork.IsConnected) photonView.RPC(nameof(PlayWallHitEffect), RpcTarget.All);
				else PlayWallHitEffect();
			}

			currentHits++;

			if (animator) {
				animator.SetTrigger("Hit");
			}

			//TODO: Add some shake settings
			GameManager.Instance.mainCamera.DOShakePosition(0.5f, 0.5f, 2);
			OnHit?.Invoke(this);
		}		
	}

	[PunRPC]
	private void PlayWallHitEffect() {
		surfaceType.effectCollection.Get("BackWallHit")?.Play(currentBall.transform.position, Vector3.zero);
	}
	
	[PunRPC]
	protected override void PlayBounceEffect(Vector3 position) {
		//
	}

	public void ResetFloorColor() {
		if (floorRenderer) {
			floorRenderer.material.color = startingColor;
		}
	}

	public void SetFloorColor(float amount) {
		if (floorRenderer) {
			floorRenderer.material.color = Color.Lerp(startingColor, damageColor, amount);
		}
	}

	public bool OnHitClear() {
		OnHit = null;
		return true;
	}

	public void SetActive(bool value) {
		isActive = value;
	}
}
