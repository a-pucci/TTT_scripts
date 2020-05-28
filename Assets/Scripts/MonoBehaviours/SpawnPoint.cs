using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	public PlayerNumber player;
	[HideInInspector] public Status ballStatus;
	
	private Animator animator;

	private void Start() {
		animator = GetComponent<Animator>();
	}

	public void StartSpawnAnimation() {
		animator.SetTrigger("SpawnBall");
	}

	public void SpawnBall() {
		Vector3 spawnDirection = GameManager.Instance.ballSpawner.spawnDirection;
		Vector3 ballSpawnVelocity = (player == PlayerNumber.Top ? spawnDirection : new Vector3(-spawnDirection.x, spawnDirection.y, -spawnDirection.z)) * GameManager.Instance.ballSpawner.spawnSpeed;
		if (GameManager.Instance.ballSpawner.currentBall == null) {
			GameManager.Instance.ballSpawner.currentBall = Instantiate(GameManager.Instance.ballSpawner.regularBallPrefab, transform.position, Quaternion.identity);
		}
		else {
			BallPhysics ball = GameManager.Instance.ballSpawner.currentBall.GetComponent<BallPhysics>();
			ball.ToggleRenderer(true);
			ball.stopped = false;
		}
		GameManager.Instance.ballSpawner.lastPlayerDirectionSpawn = player;
		var ballPhysics = GameManager.Instance.ballSpawner.currentBall.GetComponent<BallPhysics>();
		ballPhysics.canDamage = false;
		ballPhysics.velocity = ballSpawnVelocity;
		if (ballStatus) {
			ballStatus.OnApply(ballPhysics);
		}
		AudioManager.instance.PlaySound(GameManager.Instance.ballSpawner.spawnAudio, false, .1f);
	}

	public PlayerNumber GetPlayer() {
		return player;
		//
	}
}