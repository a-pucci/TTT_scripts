using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScoreCityArena : Score {

	#region Fields

	[Title("Top Player")] public ScorePointsMeshes topPoints;

	[Title("Bottom Player")] public ScorePointsMeshes bottomPoints;
	
	#endregion

	#region Unity Callbacks

	protected override void Start() {
		base.Start();
		
		currentTopPointPosition = topPoints.points[0].transform.position;
		currentBottomPointPosition = bottomPoints.points[0].transform.position;
	}

	#endregion
	
	#region Methods
	protected override void UpdateScore(Dictionary<PlayerPosition, int> currentScore, PlayerPosition player) {
		base.UpdateScore(currentScore, player);
		
		ResetPoints(bottomPoints.points, out bottomPointCounter, PlayerPosition.Bottom);
		ResetPoints(topPoints.points, out topPointCounter, PlayerPosition.Top);
	}
	
	protected override void DecreasePoint(PlayerPosition player) {
		var target = new Vector3();
		
		if (topPointCounter <= topPoints.points.Count && bottomPointCounter <= bottomPoints.points.Count) {
			switch (player) {
				case PlayerPosition.Bottom:
					target = currentBottomPointPosition;
					bottomPoints.points[bottomPointCounter].material = bottomPoints.inactiveMaterial;
					bottomPointCounter++;
					currentBottomPointPosition = bottomPointCounter < bottomPoints.points.Count ? bottomPoints.points[bottomPointCounter].transform.position : bottomPoints.points[0].transform.position;
					break;
				
				case PlayerPosition.Top:
					target = currentTopPointPosition;
					topPoints.points[topPointCounter].material = topPoints.inactiveMaterial;
					topPointCounter++;
					currentTopPointPosition = topPointCounter < topPoints.points.Count ? topPoints.points[topPointCounter].transform.position : topPoints.points[0].transform.position;
					break;
			}	
		}
		Vector3 source = FindObjectOfType<BallPhysics>().transform.position;
		if (lightninghEnabled) {
			lightning.CreateLightning(source, target);
		}
	}
	
	protected override void ResetPoints<T>(List<T> points, out int counter, PlayerPosition player) {
		var pointsObj = points as List<MeshRenderer>;
		foreach (MeshRenderer point in pointsObj) {
			point.material = player == PlayerPosition.Top ? topPoints.activeMaterial : bottomPoints.activeMaterial;
		}
		counter = 0;
	}
	
	#endregion

}
[Serializable]
public class ScorePointsMeshes {
	public List<MeshRenderer> points;
	public Material activeMaterial;
	public Material inactiveMaterial;
}