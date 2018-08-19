/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:		Procedural Maze Generation
Date:			9/27/2017
Class:			GMD 300
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MazeGenerator))]
public class LevelManager : MonoBehaviour {

	public static LevelManager instance;

	private Grid pathfindingGrid;

	public GameObject startingRoomPrefab;
	public GameObject playerPrefab;
	public GameObject endPrefab;
	public GameObject[] collectablePrefabs;
	public MazeData mazeData;

	private Transform player;
	private int pathsRequested;
	private float highestDistance;
	private int highestDistanceIndex;

	private void Awake () {
		instance = GetComponent<LevelManager> ();
		if (pathfindingGrid == null) {
			pathfindingGrid = FindObjectOfType<Grid> ();
		}
	}

	public void GenerateLevel (MazeData mazeData) {

		if (pathfindingGrid == null) {
			pathfindingGrid = FindObjectOfType<Grid> ();
		}

		float spacing = mazeData.scale - (mazeData.scale / 10f);
		Vector2 gridWorldSize = new Vector2 (mazeData.mazeSize.x, mazeData.mazeSize.y);
		gridWorldSize *= spacing;
		pathfindingGrid.SetUpPathfinding (gridWorldSize, spacing / 10f / 2f);

		this.mazeData = mazeData;
		StartCoroutine (CalculateDistanceFromStart ());
	}

	public void OnDistanceFound (float distance, int index) {
		pathsRequested--;
		if (distance > highestDistance) {
			highestDistance = distance;
			highestDistanceIndex = index;
		}
	}

	private void SpawnPrefabs () {
		float spacing = mazeData.scale - (mazeData.scale / 10f);
		for (int i = 0; i < mazeData.specialCells.Count; i++) {
			if (i == 0) {
				Vector3 startRoomPosition = mazeData.specialCells [0].mazeCell.position - Vector3.right * ((spacing + mazeData.scale / 10f) / 2f) - Vector3.up * (spacing / 2f) + Vector3.forward * ((mazeData.scale / 10f) / 2f);
				GameObject startRoom = Instantiate (startingRoomPrefab, startRoomPosition, Quaternion.identity);
				startRoom.transform.localScale = Vector3.one * mazeData.scale;
				player = Instantiate (playerPrefab, startRoom.transform.GetChild (0).position, Quaternion.identity).transform;
			}
			if (i == highestDistanceIndex) {
				Instantiate (endPrefab, mazeData.specialCells [i].mazeCell.position + Vector3.up, Quaternion.identity);
			}
		}
	}

	private IEnumerator CalculateDistanceFromStart () {
		if (Time.timeSinceLevelLoad < 0.3f) {
			yield return new WaitForSeconds (0.3f);
		}

		Vector3 startPosition = mazeData.specialCells [0].mazeCell.position;
		for (int i = 0; i < mazeData.specialCells.Count; i++) {
			if (i > 0) {
				pathsRequested++;
				mazeData.specialCells [i].RequestPath (startPosition);
			}
		}

		while (pathsRequested > 0) {
			yield return null;
		}

		mazeData.specialCells [highestDistanceIndex].cellType = CellType.Finish;

		for (int i = 0; i < mazeData.specialCells.Count; i++) {
			if (i > 0 && i != highestDistanceIndex) {
				mazeData.specialCells [i].cellType = (mazeData.prng.Next (0, 2) == 0) ? CellType.Enemy : CellType.Collectable;
			}
		}

		SpawnPrefabs ();
	}
}