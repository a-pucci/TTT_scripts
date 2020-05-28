using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GreekArenaManager : MonoBehaviourPun {

	public CharacterManager topPlayer;
	public CharacterManager bottomPlayer;
	public BallSpawner ballSpawner;
	public EffectCollection effects;

	[BoxGroup("Event Canvas Settings")] public bool showCanvas;
	[BoxGroup("Event Canvas Settings")] public float canvasWaitBeforeShow;
	[BoxGroup("Event Canvas Settings")] public float canvasTime;
	[BoxGroup("Event Canvas Settings")] public GameObject canvas;
	[BoxGroup("Event Canvas Settings")] public TextMeshProUGUI canvasText;
 
	public List<Status> statuses;
	[BoxGroup("White Traps Settings")] public List<MeshRenderer> whiteTiles;
	[BoxGroup("White Traps Settings")] public Material activeWhiteMaterial;

	[Space(20)] 
	[BoxGroup("Black Traps Settings")] public float timeBeforeTrapsActivation;
	[BoxGroup("Black Traps Settings")] public float timeBetweenRows;
	[BoxGroup("Black Traps Settings")] public float trapsStunTime;
	[BoxGroup("Black Traps Settings")] public Material onActivationBlackTrapMaterial;
	[BoxGroup("Black Traps Settings")] public Material activeBlackTrapMaterial;
	
	[ReadOnly, FoldoutGroup("Editor")]public List<TrapSurface> traps;
	[ReadOnly, FoldoutGroup("Editor")]public List<MeshRenderer> bottomBlackTiles;
	[ReadOnly, FoldoutGroup("Editor")]public List<MeshRenderer> topBlackTiles;

	private Material blackTilesMaterial;
	private Material whiteTilesMaterial;
	private bool gameOver;
	
	private void Start () {
		if (!PhotonNetwork.IsConnected || photonView.IsMine) {
			FindObjectOfType<GameManager>().ScoreIncreased += ChangeEvent;
		}
		FindObjectOfType<GameManager>().GameOver += GameOver;
		blackTilesMaterial = bottomBlackTiles[0].material;
		whiteTilesMaterial = whiteTiles[0].material;
	}

	private void GameOver() {
		gameOver = true;
		Reset();
		StopAllCoroutines();
	}

	private void ChangeEvent(Dictionary<PlayerPosition, int> currentScore, PlayerPosition player) {
		Reset();
		if (!gameOver) {
			int random = Random.Range(0, 2);
			switch (random) {
				case 0:
					if (PhotonNetwork.IsConnected) {
						photonView.RPC(nameof(WhiteEvent), RpcTarget.All);
					}
					else {
						WhiteEvent();
					}
					break;
				case 1:
					if (PhotonNetwork.IsConnected) {
						photonView.RPC(nameof(BlackEvent), RpcTarget.All);
					}
					else {
						BlackEvent();
					}
					break;
			}
		}
	}

	[PunRPC]
	private void WhiteEvent() {
		if (showCanvas) {
			StartCoroutine(ShowEventCanvas("WHITE"));
		}
		effects.Get("WhiteEvent")?.Play(transform.position, Vector3.zero);
		whiteTiles.ForEach(x => x.material = activeWhiteMaterial);
		
		Status movementReduction = statuses.Find(x => x.GetType() == typeof(MovementReduction));
		movementReduction.OnApply(topPlayer);
		movementReduction.OnApply(bottomPlayer);
		
		Status dashIncrement = statuses.Find(x => x.GetType() == typeof(DashIncrement));
		dashIncrement.OnApply(topPlayer);
		dashIncrement.OnApply(bottomPlayer);
		
		Status sizeIncrement = statuses.Find(x => x.GetType() == typeof(SizeIncrement));
		sizeIncrement.OnApply(ballSpawner.currentBall.GetComponent<BallPhysics>());
		ballSpawner.topSpawn.ballStatus = sizeIncrement;
		ballSpawner.bottomSpawn.ballStatus = sizeIncrement;
	}

	[PunRPC]
	private void BlackEvent() {
		if (showCanvas) {
			StartCoroutine(ShowEventCanvas("BLACK"));
		}
		effects.Get("BlackEvent")?.Play(transform.position, Vector3.zero);
		StartCoroutine(TrapsActivation());
	}

	private IEnumerator TrapsActivation() {
		// Wait before traps activate
		yield return new WaitForSeconds(timeBeforeTrapsActivation);
		
		// Each time change material to 1 black row per side
		for (int i = 0; i < 6; i+=2) {
			effects.Get("RowActivation")?.Play(transform.position, Vector3.zero);
			
			bottomBlackTiles[i].material = onActivationBlackTrapMaterial;
			bottomBlackTiles[i+1].material = onActivationBlackTrapMaterial;
			topBlackTiles[i].material = onActivationBlackTrapMaterial;
			topBlackTiles[i + 1].material = onActivationBlackTrapMaterial;
			yield return new WaitForSeconds(timeBetweenRows);
		}

		// Change material to active traps
		bottomBlackTiles.ForEach(x => x.material = activeBlackTrapMaterial);
		topBlackTiles.ForEach(x => x.material = activeBlackTrapMaterial);
	
		// Traps stun
		traps.ForEach(x => x.Stun(trapsStunTime));
		effects.Get("TrapStun")?.Play(transform.position, Vector3.zero);
		yield return new WaitForSeconds(trapsStunTime);

		// After stun set material to initial black material
		bottomBlackTiles.ForEach(x => x.material = blackTilesMaterial);
		topBlackTiles.ForEach(x => x.material = blackTilesMaterial);
	}

	private void Reset() {
		whiteTiles.ForEach(x => x.material = whiteTilesMaterial);
		
		Status movementReduction = statuses.Find(x => x.GetType() == typeof(MovementReduction));
		movementReduction.OnRemove(topPlayer);
		movementReduction.OnRemove(bottomPlayer);
		
		Status dashIncrement = statuses.Find(x => x.GetType() == typeof(DashIncrement));
		dashIncrement.OnRemove(topPlayer);
		dashIncrement.OnRemove(bottomPlayer);
		
		Status sizeIncrement = statuses.Find(x => x.GetType() == typeof(SizeIncrement));
		sizeIncrement.OnRemove(ballSpawner.currentBall.GetComponent<BallPhysics>());
		ballSpawner.topSpawn.ballStatus = null;
		ballSpawner.bottomSpawn.ballStatus = null;
	}

	private IEnumerator ShowEventCanvas(string eventName) {
		yield return new WaitForSeconds(canvasWaitBeforeShow);
		canvas.SetActive(true);
		canvasText.text = eventName;
		yield return new WaitForSeconds(canvasTime);
		canvas.SetActive(false);
	}
	
	
	#if UNITY_EDITOR
	[PropertySpace(20), Button(ButtonSizes.Medium), FoldoutGroup("Editor")]
	private void GetAllStatuses() {
		statuses = GetAllInstances<Status>().ToList();
	}
	
	private static T[] GetAllInstances<T>() where T : ScriptableObject {
		string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
		var a = new T[guids.Length];
		for (int i = 0; i < guids.Length; i++) {
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
		}
		return a;
	}
	
	[Button(ButtonSizes.Medium), FoldoutGroup("Editor")]
	private void GetAllTraps() {
		traps = GetAllReferences<TrapSurface>().ToList();
		traps = traps.OrderBy(x => x.name).ToList();
		topBlackTiles = new List<MeshRenderer>();
		bottomBlackTiles = new List<MeshRenderer>();

		for (int i = traps.Count / 2 -1; i >= 0; i--) {
			topBlackTiles.Add(traps[i].GetComponent<MeshRenderer>());
		}
		
		for (int i = traps.Count/2; i < traps.Count ; i++) {
			bottomBlackTiles.Add(traps[i].GetComponent<MeshRenderer>());	
		}
	}
	
	private static T[] GetAllReferences<T>() where T : MonoBehaviour {
		T[] a = Resources.FindObjectsOfTypeAll<T>();
		return a;
	}
	
	#endif
}