using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	#region Fields

	// Public
	public Image topScore;
	public Image bottomScore;
	public SpriteCollection scores;

	public bool lightninghEnabled = true;

	protected Lightning lightning;
	protected Vector3 currentTopPointPosition;
	protected Vector3 currentBottomPointPosition;
	protected int topPointCounter;
	protected int bottomPointCounter;

	#endregion

	#region Unity Callbacks

	protected virtual void Start () {
		FindObjectOfType<GameManager>().ScoreIncreased += UpdateScore;
		FindObjectOfType<GameManager>().WallHit += DecreasePoint;
		lightning = GetComponent<Lightning>();
	}
	#endregion

	#region Methods

	protected virtual void UpdateScore(Dictionary<PlayerPosition, int> currentScore, PlayerPosition player) {
		// TODO top and bottom are inverted 
		topScore.sprite = scores.collection[currentScore[PlayerPosition.Bottom]];
		bottomScore.sprite = scores.collection[currentScore[PlayerPosition.Top]];
	}

	protected virtual void DecreasePoint(PlayerPosition player) {
		// To be Implemented
	}

	protected virtual void ResetPoints<T>(List<T> points, out int counter, PlayerPosition player) {
		counter = 0;
	}

	#endregion

}