using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class ScoreGreekArena : Score {

	#region Fields

	public SpriteCollection numbers;
	public Image topNumber;
	public Image bottomNumber;
	
	public SpriteCollection numbersToColor;
    public Image topColoredNumber;
    public Image bottomColoredNumber;

	public Color topColor;
	public Color bottomColor;
    	
	[Title("Top Player")] public ScorePointsMeshes topPoints;
	[Title("Bottom Player")] public ScorePointsMeshes bottomPoints;
	

	#endregion

	#region Unity Callbacks

	protected override void Start() {
		base.Start();
		topColoredNumber.color = topColor;
		bottomColoredNumber.color = bottomColor;
		currentTopPointPosition = topPoints.points[0].transform.position;
		currentBottomPointPosition = bottomPoints.points[0].transform.position;
	}
	
	#endregion

	#region Methods

	protected override void UpdateScore(Dictionary<PlayerPosition, int> currentScore, PlayerPosition player) {
		base.UpdateScore(currentScore, player);
		
		topNumber.sprite = numbers.collection[currentScore[PlayerPosition.Bottom]];
		bottomNumber.sprite = numbers.collection[currentScore[PlayerPosition.Top]];
		
		topColoredNumber.sprite = numbersToColor.collection[currentScore[PlayerPosition.Bottom]];
		topColoredNumber.color = topColor;
		bottomColoredNumber.sprite = numbersToColor.collection[currentScore[PlayerPosition.Top]];
		bottomColoredNumber.color = bottomColor;
		
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