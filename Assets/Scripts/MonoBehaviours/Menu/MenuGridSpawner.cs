using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MenuGridSpawner : MonoBehaviour {

	public GameObject gridPrefab;
	public Transform grid;
	public Transform spawnPoint;

	public int gridRows;
	public float gridSpeed;

	public float offsetZ = 0.06f;

	private Vector3 speed;
	private Vector3 prefabSize;
	private float distance;
	private Transform lastRow;

	private void Start() {
		foreach (Transform child in grid.transform) {
			Destroy(child.gameObject);
		}
		SpawnGrid();
	}

	private void Update() {
		distance = Vector3.Distance(spawnPoint.position, lastRow.position);
		if (distance >= (prefabSize.z - offsetZ)) {
			SpawnRow(spawnPoint.position);
		}
	}

	[Button]
	private void SpawnGrid() {
		
		SpawnRow(grid.position, true);
		for (int i = 1; i < gridRows; i++) {
			SpawnRow(new Vector3(grid.position.x, grid.position.y, grid.position.z + (prefabSize.z - offsetZ)*i));
		}
		SetSpawnPoint();
	}

	private void SpawnRow(Vector3 position, bool first = false) {
	
		GameObject row = Instantiate(gridPrefab, position, Quaternion.identity, grid);
		if (first) {
			prefabSize = row.GetComponent<MenuGrid>().Size();
			speed = new Vector3(0, 0, -gridSpeed);
		}
		row.GetComponent<Rigidbody>().velocity = speed;

		lastRow = row.transform;
	}

	[Button]
	private void SetSpawnPoint() { 
		spawnPoint.position = new Vector3(grid.position.x, grid.position.y, grid.position.z + (prefabSize.z - offsetZ) * gridRows);
	}
}