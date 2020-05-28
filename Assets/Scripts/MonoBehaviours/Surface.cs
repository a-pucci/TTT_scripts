using Photon.Pun;
using UnityEngine;

public class Surface : MonoBehaviourPun {

	#region Fields

	// Public
	public SurfaceType surfaceType;
	private Vector3 hitPosition;

	#endregion
	
	#region Methods

	public virtual void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Character")) {
			WalkOn(other.gameObject.GetComponent<CharacterManager>());
		}
	}

	public virtual void RaycastHit(BallPhysics ball, RaycastHit hitInfo) {
		if (surfaceType.consideredForBounces) {
			ball.currentBounces++;
		}
		ball.velocity = Vector3.Reflect(ball.velocity, hitInfo.normal) * surfaceType.dampening;
		if (/*ball.currentBounces >= ball.ballType.bouncesUntilRespawn || */ball.velocity.magnitude <= ball.ballType.minVelocityToRespawn) {
			ball.ToggleRenderer(false);
			ball.stopped = true;
			//Debug.Log("Ball DESTROYED after " + ball.ballType.bouncesUntilRespawn.ToString());
			//TODO: Handle ball spawning
			if (!GameManager.Instance.gameOver) {
				StartCoroutine(GameManager.Instance.ballSpawner.WaitBeforeBallSpawn(GameManager.Instance.ballSpawner.spawnTime));	
			}
		}		
		if (surfaceType.effectCollection != null) {
			if(PhotonNetwork.IsConnected) photonView.RPC(nameof(PlayBounceEffect), RpcTarget.All, hitInfo.point);
			else PlayBounceEffect(hitInfo.point);
		}
	}

	[PunRPC]
	protected virtual void PlayBounceEffect(Vector3 position) {
		surfaceType.effectCollection.Get("Bounce")?.Play(position, Vector3.zero);
	}

	public virtual void TriggerHit(BallPhysics ball) { }

	public virtual void WalkOn(CharacterManager character) { }

	#endregion

}