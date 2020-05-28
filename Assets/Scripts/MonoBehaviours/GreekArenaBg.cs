using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityScript.Steps;

public class GreekArenaBg : MonoBehaviour {
	public Transform blackTiles;
	public Transform whiteTiles;

	public float yOffset;
	public float speed;
	
	private Vector3 endPosUp;
	private Vector3 endPosDown;

	private bool backwardsWhite;
	private bool backwardsBlack = true;
	

	private void Start() {
		endPosDown = new Vector3(blackTiles.position.x, blackTiles.position.y - yOffset, blackTiles.position.z);
		endPosUp = new Vector3(blackTiles.position.x, blackTiles.position.y + yOffset, blackTiles.position.z);
	}

	// Update is called once per frame
	public void Update() {
		
		UpdateTiles(blackTiles, ref backwardsBlack);
		UpdateTiles(whiteTiles, ref backwardsWhite);
	}

	private void UpdateTiles(Transform tile, ref bool backwards) {
		float initialY = backwards ? endPosDown.y : endPosUp.y;
		float endY = backwards ? endPosUp.y : endPosDown.y;
		float y = Mathf.Lerp(initialY, endY, Mathf.InverseLerp(initialY, endY, tile.position.y) + speed * .01f);

		var temp = new Vector3(tile.position.x, y, tile.position.z);
		tile.position = temp;

		if (y == endY) {
			backwards = !backwards;
		}
	}
}
