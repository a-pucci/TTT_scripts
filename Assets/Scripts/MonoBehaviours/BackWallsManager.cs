using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public enum WallType {
	Center,
	Side
}

public class BackWallsManager : MonoBehaviour {

	private GameManager gameManager;

	public float swapWaitTime;
	public Walls topWalls;
	public Walls bottomWalls;
	

	private WallType currentTopWall;
	private WallType currentBottomWall;

	private void Start () {
		gameManager = GameManager.Instance;
		gameManager.WallHit += OnWallHit;
		currentTopWall = WallType.Center;
		currentBottomWall = WallType.Center;
		SwapWalls(topWalls, true);
		SwapWalls(bottomWalls, true);
		
		gameManager.AddBackWalls(topWalls.GetAllWalls());
		gameManager.AddBackWalls(bottomWalls.GetAllWalls());
		
		ChangeWallsMaterial(topWalls, currentTopWall, PlayerPosition.Top);
		ChangeWallsMaterial(bottomWalls, currentBottomWall, PlayerPosition.Bottom);
	}

	private void HandleSwap(PlayerPosition player) {
		if (player == PlayerPosition.Bottom) {
			currentBottomWall = InvertType(currentBottomWall);
			SwapWalls(bottomWalls, currentBottomWall == WallType.Center);
			ChangeWallsMaterial(bottomWalls, currentBottomWall, player);
		}
		else {
			currentTopWall = InvertType(currentTopWall);
			SwapWalls(topWalls, currentTopWall == WallType.Center);		
			ChangeWallsMaterial(topWalls, currentTopWall, player);
		}
	}

	private void OnWallHit(PlayerPosition player) {
		StartCoroutine(WaitToSwap(player));
	}
	
	private IEnumerator WaitToSwap(PlayerPosition player) {
		yield return new WaitForSeconds(swapWaitTime);
		HandleSwap(player);
	}

	private void SwapWalls(Walls walls, bool isCenter) {
		foreach (BackWall wall in walls.sides) {
			wall.SetActive(!isCenter);
		}
		walls.center.SetActive(isCenter);
	}
	
	private void ChangeWallsMaterial(Walls walls, WallType activeType, PlayerPosition player) {
		walls.center.gameObject.GetComponent<MeshRenderer>().material = activeType == WallType.Center ? walls.activeMaterial : walls.inactiveMaterial;

		foreach (BackWall wall in walls.sides) {
			wall.gameObject.GetComponent<MeshRenderer>().material = activeType == WallType.Side ? walls.activeMaterial : walls.inactiveMaterial;
		}
	}
	
	private WallType InvertType(WallType type) {
		return type == WallType.Center ? WallType.Side : WallType.Center;
	}

	public void SetMaterials(Material active, Material inactive, PlayerPosition player) {
		if (player == PlayerPosition.Top) {
			topWalls.activeMaterial = active;
			topWalls.inactiveMaterial = inactive;
		}
		else {
			bottomWalls.activeMaterial = active;
			bottomWalls.inactiveMaterial = inactive;
		}
	}
}

[Serializable]
public class Walls {
	public BackWall center;
	public List<BackWall> sides;
	public Material activeMaterial;
	public Material inactiveMaterial;

	public List<BackWall> GetAllWalls() {
		var list = new List<BackWall> {center};
		list = list.Union(sides).ToList();
		return list;
	} 
}
