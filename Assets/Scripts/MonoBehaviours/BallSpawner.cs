using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BallSpawner : MonoBehaviourPun {

	#region Fields

	// Public
	public float spawnTime;
	public float longSpawnTime;
	public SpawnPoint topSpawn;
	public SpawnPoint bottomSpawn;
	public Vector3 spawnDirection;
	public float spawnSpeed;
	public GameObject regularBallPrefab;
	public GameObject killerBallPrefab;
	public AudioClip spawnAudio;
	[HideInInspector] public Status ballStatus;
	

	// Public Hidden
	[HideInInspector]
	public GameObject currentBall;
	[HideInInspector]
	public PlayerNumber lastPlayerDirectionSpawn;

	// Private
	private bool canSpawn;
	private bool masterIntroEnded;
	private bool clientIntroEnded; 
	
	// Properties

	// Components

	// Events
	public event Action BallSpawned;
	
	#endregion

	#region Unity Callbacks

	//private void Awake() {
	//	canSpawn = true;
		
	//	//lastPlayerDirectionSpawn = playerSpawn;
	//	StartCoroutine(WaitBeforeBallSpawn(longSpawnTime));
	//}

	#endregion

	#region Methods

	public void StartSpawning() {
		canSpawn = true;
		currentBall = GameObject.FindGameObjectWithTag("Ball");
		//lastPlayerDirectionSpawn = playerSpawn;
		StartCoroutine(WaitBeforeBallSpawn(longSpawnTime));
	}

	//Spawns the ball towards a certain player
	public void SpawnBall(PlayerNumber player) {

		SpawnPoint spawnPoint = player == topSpawn.GetPlayer() ? topSpawn : bottomSpawn;

		if (canSpawn) {
			//Debug.Log("Ball Spawned at " + ballSpawnPosition.position);
			spawnPoint.StartSpawnAnimation();			
			BallSpawned?.Invoke();
		}
	}

	private PlayerNumber GetPlayerSpawn(PlayerNumber player) {
		if (currentBall == null) {
			//If no player has been decided (first spawn/spawn on ball self destruct)
			if (player == PlayerNumber.None) {
				//Checks last spawn
				switch (lastPlayerDirectionSpawn) {
					//Ball was never spawned before
					case PlayerNumber.None:
						//Picks a player at random
						player = UnityEngine.Random.Range(0,2) == 1 ? PlayerNumber.Top : PlayerNumber.Bottom;
						break;
					//Picks the opposite player to the last one towards which it spawned
					case PlayerNumber.Bottom:
						player = PlayerNumber.Top;
						break;
					case PlayerNumber.Top:
						player = PlayerNumber.Bottom;
						break;
				}
			}
		}
		return player;
	}
	
	public GameObject GetCurrentBall() {
		return currentBall;
	}
	
	// Waits a set amount of time then spawns the ball. BUG: make this work when the ball is destroyed
	public IEnumerator WaitBeforeBallSpawn(float waitTime, PlayerNumber player = PlayerNumber.None) {
		GameManager.Instance.ballSpawner.currentBall.transform.position = (player == topSpawn.GetPlayer() ? topSpawn : bottomSpawn).transform.position;
		PlayerNumber newPlayer = GetPlayerSpawn(player);
		yield return new WaitForSeconds(waitTime);
		
		if (PhotonNetwork.IsConnected) {
			yield return new WaitUntil(() => masterIntroEnded && clientIntroEnded);
		}
		
		SpawnBall(newPlayer);
	}
	
	public void DestroyCurrentBall() {
		//Destroy(currentBall);
		BallPhysics ball = currentBall.GetComponent<BallPhysics>();
		ball.ToggleRenderer(false);
		ball.stopped = true;
		//canSpawn = false;
	}

	[PunRPC]
	public void AnimationCompleted(bool isMaster) {
		if (isMaster) {
			masterIntroEnded = true;
		}
		else {
			clientIntroEnded = true;
		}
	}
	
	
	#endregion

}