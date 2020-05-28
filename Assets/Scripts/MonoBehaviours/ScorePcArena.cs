using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePcArena : Score {

	#region Fields
	public List<GameObject> topPoints;
	public List<GameObject> bottomPoints;
	public VisualEffect topPointsExplosion;
	public VisualEffect bottomPointsExplosion;
	
	#endregion

	#region Unity Callbacks

	protected override void Start() {
		base.Start();
		
		currentTopPointPosition = topPoints[0].transform.position;
		currentBottomPointPosition = bottomPoints[0].transform.position;
	}

	#endregion

	#region Methods
	protected override void UpdateScore(Dictionary<PlayerPosition, int> currentScore, PlayerPosition player) {
		base.UpdateScore(currentScore, player);
		
		ResetPoints(bottomPoints, out bottomPointCounter, player);
		ResetPoints(topPoints, out topPointCounter, player);
	}
	
	protected override void DecreasePoint(PlayerPosition player) {
		var target = new Vector3();

		if (topPointCounter <= topPoints.Count && bottomPointCounter <= bottomPoints.Count) {
			switch (player) {
				case PlayerPosition.Bottom:
					target = currentBottomPointPosition;
					bottomPoints[bottomPointCounter].SetActive(false);
					bottomPointsExplosion.Play(bottomPoints[bottomPointCounter].transform.position, Vector3.zero);
					bottomPointCounter++;
					currentBottomPointPosition = bottomPointCounter < bottomPoints.Count ? bottomPoints[bottomPointCounter].transform.position : bottomPoints[0].transform.position;
					break;
				
				case PlayerPosition.Top:
					target = currentTopPointPosition;
					topPoints[topPointCounter].SetActive(false);
					topPointsExplosion.Play(topPoints[topPointCounter].transform.position, Vector3.zero);
					topPointCounter++;
					currentTopPointPosition = topPointCounter < topPoints.Count ? topPoints[topPointCounter].transform.position : topPoints[0].transform.position;
					break;
			}	
		}
		
		Vector3 source = FindObjectOfType<BallPhysics>().transform.position;
		if (lightninghEnabled) {
			lightning.CreateLightning(source, target);
		}
	}
	
	protected override void ResetPoints<T>(List<T> points, out int counter, PlayerPosition player) {
		var pointsObj = points as List<GameObject>;
		foreach (GameObject point in pointsObj) {
			point.SetActive(true);
		}
		counter = 0;
	}
	
	#endregion
}