using System;
using Photon.Pun;
using UnityEngine;

public class RacketSurface : Surface {

	#region Fields

	// Public
	public CharacterManager character;

	private BoxCollider boxCollider;

	#endregion

	#region Unity Callbacks

	private void Awake () {
		character = character ?? transform.parent.GetComponent<CharacterManager>();
		boxCollider = GetComponent<BoxCollider>();
	}

	// public override void OnTriggerEnter(Collider other) {
	// 	if (other.gameObject.CompareTag("Ball")) {
	// 		TriggerHit(other.gameObject.GetComponent<BallPhysics>());
	// 	}
	// }

	#endregion

	#region Methods

	public override void TriggerHit(BallPhysics ball) {
		if (character.playerStatus == PlayerStatus.Dashing) {
			GameAnalytics.AddEvent(character.playerPosition, EventKeys.DashAndHit);
		}
		if (character.currentSwingStrength == character.swingType.strongSwing) {
			GameAnalytics.AddEvent(character.playerPosition, EventKeys.SuccessfulChargedStrike);
		}
		if (character.currentSwingStrength == character.swingType.lightSwing) {
			GameAnalytics.AddEvent(character.playerPosition, EventKeys.FailedChargedStrike);
		}
		GameAnalytics.AddEvent(character.playerPosition, EventKeys.HitBall);

		float multiplier = (ball.transform.position.y > transform.position.y ?
			Mathf.Lerp(character.swingType.highSwingYMultiplier, 1, Mathf.InverseLerp(transform.position.y + (boxCollider.size.y * 0.5f), transform.position.y, ball.transform.position.y)) :
			Mathf.Lerp(character.swingType.lowSwingYMultiplier, 1, Mathf.InverseLerp(transform.position.y - (boxCollider.size.y * 0.5f), transform.position.y, ball.transform.position.y)));

		float swingY = character.swingType.swingY * multiplier;

		Vector3 newDirection = (Vector3.right * character.input.x * character.swingType.swingX) + 
		                       (Vector3.up * swingY) + 
		                       ((character.playerPosition == PlayerPosition.Top ? Vector3.back : Vector3.forward) * character.currentSwingStrength);
		ball.photonView.RequestOwnership();
		ball.currentBounces = 0;
		ball.ballSwingPower = character.currentCharge;

		//#region Swing VFX
		//switch (character.currentCharge) {
		//	case SwingPower.Regular:
		//		AudioManager.instance.PlaySound(ball.ballType.ballHitSound,false,0.8f);
		//		vfx = ball.ballType.mediumShotEffect;
		//		break;
		//	case SwingPower.Strong:
		//		AudioManager.instance.PlaySound(ball.ballType.ballHitSoundHard,false, 0.8f);
		//		vfx = ball.ballType.strongShotEffect;
		//		break;
		//	default:
		//		AudioManager.instance.PlaySound(ball.ballType.ballHitSound,false);
		//		vfx = ball.ballType.weakShotEffect;
		//		break;
		//}

		//vfx.Play(ball.transform.position, Vector3.zero);
		//#endregion

		ball.velocity = ball.transform.position + newDirection;
		if(PhotonNetwork.IsConnected) ball.photonView.RPC(nameof(ball.PlayHitEffect), RpcTarget.All, character.currentCharge.ToString(), ball.transform.position);
		else ball.PlayHitEffect(character.currentCharge.ToString(), ball.transform.position);
	}


	
	[PunRPC]
	protected override void PlayBounceEffect(Vector3 position) {
		//
	}

	#endregion

}