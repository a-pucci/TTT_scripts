using System;
using System.Collections.Generic;
using System.Linq;
using Kino;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using System.Collections;
using Photon.Pun;

public class GameManager : MonoBehaviourPun {

	public bool canRestart;
	public int backWallMaxHits;
	public int scoreIncrease;
	public int killerPointScore;
	public int winningScore;
	[BoxGroup("Game Over Settings")]
	public RectAnchorAnimation backgroundGameOverAnimation;
	[BoxGroup("Game Over Settings")]
	public Vector3 cameraOffset;
	[BoxGroup("Game Over Settings")]
	public float waitBeforeCameraZoom;
	[BoxGroup("Game Over Settings")]
	public AnimationCurve cameraZoomCurve;
	[BoxGroup("Game Over Settings")]
	public float cameraZoomTime;
	[BoxGroup("Game Over Settings")]
	public float waitBeforeGameOverScreen;
	[BoxGroup("Game Over Settings")]
	public byte autoGameOverWinner;
	
	[HideInInspector]public BallSpawner ballSpawner;
	public List<BackWall> backWalls;
	[HideInInspector]public Camera mainCamera;
	[HideInInspector]public AnalogGlitch analogGlitch;
	[HideInInspector]public Transform net;
	[HideInInspector]public GameObject resetCanvas;
	[HideInInspector]public RectTransform backgroundImage;
	[HideInInspector]public GameObject pauseCanvas;
	[HideInInspector]public TextMeshProUGUI victoryName;
	[HideInInspector]public BigScoreAnimator scoreAnimator;
	[HideInInspector]public AnalogGlitchTransition scoreTransition;
	[HideInEditorMode, ReadOnly]
	public bool isPaused;
	[HideInEditorMode, ReadOnly]
	public bool canPause = true;
	[HideInEditorMode, ReadOnly]
	public bool gameOver;
	
	public event Action<Dictionary<PlayerPosition, int>, PlayerPosition> ScoreIncreased;
	public event Action<PlayerPosition> WallHit;
	public event Action GameOver; 
	
	[HideInEditorMode, ShowInInspector]
	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, IsReadOnly = true, KeyLabel = "PlayerPosition", ValueLabel = "BackWallHits")]
	private Dictionary<PlayerPosition, int> backWallHits;
	[HideInEditorMode, ShowInInspector]
	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, IsReadOnly = true, KeyLabel = "PlayerPosition", ValueLabel = "CurrentScore")]
	private Dictionary<PlayerPosition, int> currentScore;

	public static GameManager Instance { get; private set; }

	private Rewired.Player player1;
	private Rewired.Player player2;

	private void Awake() {
		player1 = Rewired.ReInput.players.GetPlayer(0);
		player2 = Rewired.ReInput.players.GetPlayer(1);
		if (analogGlitch == null && mainCamera != null) {
			analogGlitch = mainCamera.GetComponent<AnalogGlitch>();
		}
		if (Instance != null && Instance != this) {
			Destroy(this);
			throw new Exception("An instance of this singleton already exists.");
		}
		else {
			Instance = this;
		}
	}
	
	private void Update() {
		if (canRestart) {
			if (player1.GetButtonDown(RewiredConsts.Action.Pause) || player2.GetButtonDown(RewiredConsts.Action.Pause)) {
				Restart();
			}
		}
#if UNITY_EDITOR || INTERNAL_BUILD
		if (Input.GetKeyDown(KeyCode.R)) {
			Restart();
        }
#endif
        if ((Input.GetKeyDown(KeyCode.P) || player1.GetButtonDown(RewiredConsts.Action.Pause) || player2.GetButtonDown(RewiredConsts.Action.Pause)) && !canRestart && canPause) {
	        if (PhotonNetwork.IsConnected) {
				photonView.RPC(nameof(Pause), RpcTarget.All);
			}
			else {
				Pause();
			}
        }
		if ((Input.GetKeyDown(KeyCode.Q) || (isPaused && (player1.GetButtonDown(RewiredConsts.Action.Back) || player2.GetButtonDown(RewiredConsts.Action.BackToMenu))))) {
			isPaused = false;
			Time.timeScale = 1f;
			if (!PhotonNetwork.IsConnected) {
				SceneManager.LoadScene("scn_menu");
			}
			else {
				NetworkManager.Instance.ReturnToMenu();
			}
		}
#if UNITY_EDITOR || INTERNAL_BUILD
		autoGameOverWinner = Input.GetKeyDown(KeyCode.Alpha1) ? (byte)1 : Input.GetKeyDown(KeyCode.Alpha2) ? (byte)2 : (byte)0;
#endif
		if (autoGameOverWinner > 0) {
			if (autoGameOverWinner == 1) {
				StartCoroutine(HandleGameOver(backWalls[1].playerPosition));
			}
			if (autoGameOverWinner == 2) {
				StartCoroutine(HandleGameOver(backWalls[0].playerPosition));
			}
			autoGameOverWinner = 0;
		}
	}

	[PunRPC]
	private void Pause() {
		isPaused = !isPaused;
		pauseCanvas.SetActive(isPaused);
		Time.timeScale = (isPaused && !PhotonNetwork.IsConnected) ? 0f : 1f;
	}

	private void Start() {
		GameAnalytics.StartAnalytics();
		backWallHits = new Dictionary<PlayerPosition, int> {
			{ PlayerPosition.Top, 0 },
			{ PlayerPosition.Bottom, 0 }
		};
		currentScore = new Dictionary<PlayerPosition, int> {
			{ PlayerPosition.Top, 0 },
			{ PlayerPosition.Bottom, 0 }
		};

		backWalls.ForEach(x => x.OnHitClear());
		RegisterWallsEvents();
		SetNewArenaReferences();
	}
	
	[PunRPC]
	private void ReloadLevel() {
		NetworkManager.Instance.StopMessageQueue();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	private void Restart() {
		if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
			photonView.RPC("ReloadLevel", RpcTarget.AllViaServer);
		}
		else if (!PhotonNetwork.IsConnected) {
			ReloadLevel();
		}
	}

	private void SetNewArenaReferences() {
		var references = FindObjectOfType<GameManagerReferences>();
		if (references) {
			ballSpawner = references.ballSpawner;
			backWalls = references.backWalls;
			mainCamera = references.mainCamera;
			analogGlitch = references.analogGlitch;
			net = references.net;
			resetCanvas = references.resetCanvas;
			backgroundImage = references.backgroundImage;
			pauseCanvas = references.pauseCanvas;
			victoryName = references.victoryName;
			scoreAnimator = references.scoreAnimator;
			scoreTransition = references.scoreTransition;
		}
	}

	public void AddBackWalls(List<BackWall> newWalls) {
		backWalls = backWalls.Union(newWalls).ToList();
		RegisterWallsEvents();
	}

	public void RegisterWallsEvents() {
		foreach (BackWall backWall in backWalls) {
			backWall.OnHit += BackWallHit;
		}
	}

	
	private void BackWallHit(BackWall backWallHit) {
		if (PhotonNetwork.IsConnected && photonView.IsMine) {
			photonView.RPC(nameof(RpcBackWallHit), RpcTarget.All, (int)backWallHit.playerPosition);
		}
		else if (!PhotonNetwork.IsConnected)  {
			RpcBackWallHit((int)backWallHit.playerPosition);
		}
	}

	[PunRPC]
	private void RpcBackWallHit(int backWallHitPosition) {
		backWallHits[(PlayerPosition)backWallHitPosition]++;
		WallHit?.Invoke((PlayerPosition)backWallHitPosition);

		if (backWallHits[(PlayerPosition)backWallHitPosition] >= backWallMaxHits) {
			backWallHits[PlayerPosition.Top] = 0;
			backWallHits[PlayerPosition.Bottom] = 0;
			currentScore[(PlayerPosition)backWallHitPosition] += scoreIncrease;

			ScoreIncreased?.Invoke(currentScore, (PlayerPosition)backWallHitPosition);


			if (currentScore.Values.ToList().TrueForAll(x => x == killerPointScore)) {
				//Handle Killer Point
			}
			if (currentScore[(PlayerPosition)backWallHitPosition] >= winningScore) {
				StartCoroutine(HandleGameOver((PlayerPosition)backWallHitPosition));
			}
			ballSpawner.DestroyCurrentBall();

			//TODO: Handle ball spawning
			if (!gameOver) {
				scoreAnimator.UpdateScores(EvaluateScore(currentScore[PlayerPosition.Bottom]),
					EvaluateScore(currentScore[PlayerPosition.Top]),
					(PlayerPosition)backWallHitPosition == PlayerPosition.Bottom,
					(PlayerPosition)backWallHitPosition == PlayerPosition.Bottom,
					(PlayerPosition)backWallHitPosition == PlayerPosition.Top,
					(PlayerPosition)backWallHitPosition == PlayerPosition.Top);
				StartCoroutine(scoreTransition.Animate(Camera.main.GetComponent<AnalogGlitch>()));
				scoreAnimator.MoveScores();
				StartCoroutine(ballSpawner.WaitBeforeBallSpawn(ballSpawner.longSpawnTime));
			}
		}
	}

	public string EvaluateScore(int score) {
		switch (score) {
			case 0:
				return "00";
			case 1:
				return "15";
			case 2:
				return "30";
			case 3:
				return "40";
			default:
				return "00";
		}
	}
	
	//Enables/disables restart
	public bool ToggleRestart() {
		return canRestart = !canRestart;
	}

	//Forces restart to a certain value
	public bool ToggleRestart(bool newValue) {
		return canRestart = newValue;
	}

	public bool GetRestart() {
		return canRestart;
	}

	private void OnApplicationQuit() {
		GameAnalytics.FlushEvents();
	}

	private IEnumerator HandleGameOver(PlayerPosition backWallHitPosition) {
		gameOver = true;
		GameOver?.Invoke();
		canRestart = true;
		var references = FindObjectOfType<ArenaReferences>();
		references.topReferences.player.enabled = false;
		references.bottomReferences.player.enabled = false;
		GameAnalytics.AddEvent(backWallHitPosition, EventKeys.GameWon);
		yield return StartCoroutine(GameOverAnimation(backWallHitPosition == PlayerPosition.Top ? PlayerPosition.Bottom : PlayerPosition.Top, references));
		yield return new WaitForSeconds(waitBeforeGameOverScreen);
		StartCoroutine(backgroundGameOverAnimation.Animate(backgroundImage));
		resetCanvas.SetActive(true);
		// top and bottom are inverted
		victoryName.text = string.Format("{0} Wins!\nPress 'R' or 'Start' to Restart!", backWallHitPosition == PlayerPosition.Top ? references.bottomReferences.characterName.text : references.topReferences.characterName.text).ToUpper();
	}

	private IEnumerator GameOverAnimation(PlayerPosition winnerPosition, ArenaReferences references) {
		Animator winner, loser;
		winner = (winnerPosition == PlayerPosition.Top) ? references.topReferences.player.animator : references.bottomReferences.player.animator;
		loser = (winnerPosition == PlayerPosition.Bottom) ? references.topReferences.player.animator : references.bottomReferences.player.animator;
		winner.SetTrigger("Win");
		loser.SetTrigger("Lose");
		yield return new WaitForSeconds(waitBeforeCameraZoom);
		float t = 0;
		Vector3 startingPosition = mainCamera.transform.position;
		while (t < cameraZoomTime) {
			mainCamera.transform.position = Vector3.Lerp(startingPosition, winner.transform.position - cameraOffset, cameraZoomCurve.Evaluate(t / cameraZoomTime));
			t += Time.deltaTime;
			yield return null;
		}
	}
}
