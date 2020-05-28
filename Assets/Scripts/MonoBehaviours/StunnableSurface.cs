using System.Collections;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class StunnableSurface : Surface {

	#region Fields

	// Public
	public HitSettings hitSettings;
	public bool stunned = false;
	public SpriteRenderer sprite;
	
	// Private
	private Vector3 originalPosition;
	private CharacterManager character;

	private PhotonView photonView;

	#endregion

	#region Unity Callbacks

	private void Start () {
		originalPosition = transform.Find("Sprite").localPosition;
		character = GetComponent<CharacterManager>();
		photonView = GetComponent<PhotonView>();
	}

	#endregion

	#region Methods

	public override void RaycastHit(BallPhysics ball, RaycastHit hitInfo) {
		if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
			photonView.RPC(nameof(RpcRaycastHit), RpcTarget.All);
		}
		else {
			if (!stunned) {

				StartCoroutine(Stun(ball));

				if (hitSettings.shakeOnHit) {
					GameManager.Instance.mainCamera.DOShakePosition(hitSettings.shakeDuration, hitSettings.shakeStrength, hitSettings.shakeVibrato, 90f, true);
				}
				if (surfaceType.effectCollection) {
					surfaceType.effectCollection.Get("Hurt")?.Play(ball.transform.position, Vector3.zero);

					float stunTime = ball.ballSwingPower == SwingPower.Strong ? hitSettings.strongStunTime : hitSettings.lightStunTime;
					surfaceType.effectCollection.Get("Stun")?.Play(transform.position, Vector3.zero, sprite.transform, default(Color), stunTime);
				}
			}
		}
		base.RaycastHit(ball, hitInfo);
	}	

	public void TrapStun(float time) {
		if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
			photonView.RPC(nameof(RpcTrapStun), RpcTarget.All, time);
		}
		else {
			RpcTrapStun(time);
		}
	}
	
	private void ResetPosition() {
		sprite.gameObject.transform.localPosition = originalPosition;
	}

	private IEnumerator Stun(BallPhysics ball, float time = 0f) {

		if (ball) {
			GameAnalytics.AddEvent(character.playerPosition, EventKeys.Stunned);
			time = (ball.ballSwingPower == SwingPower.Strong ? hitSettings.strongStunTime : hitSettings.lightStunTime);
		}
		
		character.playerStatus = PlayerStatus.Stunned;
		Color oldColor = sprite.color;
		stunned = true;
		character.canMove = false;


		for (int i = 0; i < hitSettings.fadeIntervals; i++) {
			sprite.DOColor(new Color(oldColor.r, oldColor.g, oldColor.b, hitSettings.fadePercentage / 100), (time/(hitSettings.fadeIntervals*2)));
			yield return new WaitForSecondsRealtime(time / (hitSettings.fadeIntervals * 2));
			sprite.DOColor(new Color(oldColor.r, oldColor.g, oldColor.b, 1), (time / (hitSettings.fadeIntervals * 2)));
			yield return new WaitForSecondsRealtime(time / (hitSettings.fadeIntervals * 2));
		}
                character.playerStatus = PlayerStatus.None;
		stunned = false;
		character.canMove = true;
	}

	#region RPC

	[PunRPC]
	private void RpcRaycastHit() {
		if (!stunned) {

			StartCoroutine(RpcStun());

			if (hitSettings.shakeOnHit) {
				GameManager.Instance.mainCamera.DOShakePosition(hitSettings.shakeDuration, hitSettings.shakeStrength, hitSettings.shakeVibrato, 90f, true);
			}
			if (surfaceType.effectCollection) {
				surfaceType.effectCollection.Get("Hurt")?.Play(transform.position, Vector3.zero);
				surfaceType.effectCollection.Get("Stun")?.Play(transform.position, Vector3.zero, sprite.transform, default(Color), hitSettings.lightStunTime);
			}
		}
	}

	[PunRPC]
	private IEnumerator RpcStun(float time = 0f) {

		GameAnalytics.AddEvent(character.playerPosition, EventKeys.Stunned);
		time = (hitSettings.lightStunTime);

		character.playerStatus = PlayerStatus.Stunned;
		Color oldColor = sprite.color;
		stunned = true;
		character.canMove = false;


		for (int i = 0; i < hitSettings.fadeIntervals; i++) {
			sprite.DOColor(new Color(oldColor.r, oldColor.g, oldColor.b, hitSettings.fadePercentage / 100), (time / (hitSettings.fadeIntervals * 2)));
			yield return new WaitForSecondsRealtime(time / (hitSettings.fadeIntervals * 2));
			sprite.DOColor(new Color(oldColor.r, oldColor.g, oldColor.b, 1), (time / (hitSettings.fadeIntervals * 2)));
			yield return new WaitForSecondsRealtime(time / (hitSettings.fadeIntervals * 2));
		}
		character.playerStatus = PlayerStatus.None;
		stunned = false;
		character.canMove = true;
	}

	[PunRPC]
	public void RpcTrapStun(float time) {
		if (!stunned) {

			StartCoroutine(Stun(null, time));

			if (hitSettings.shakeOnHit) {
				GameManager.Instance.mainCamera.DOShakePosition(hitSettings.shakeDuration, hitSettings.shakeStrength, hitSettings.shakeVibrato, 90f, true);
			}
			if (surfaceType.effectCollection) {
				surfaceType.effectCollection.Get("Stun")?.Play(transform.position, Vector3.zero, sprite.transform, default(Color), time);
			}
		}
	}

	#endregion

	#endregion

}
