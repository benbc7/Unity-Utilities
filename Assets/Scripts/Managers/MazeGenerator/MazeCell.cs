/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:		Procedural Maze Generation
Date:			9/2/2017
Class:			GMD 300
*************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType { Enemy, Finish, Start, Collectable }

public class MazeCell {
    public Vector3 position;
    public bool visited = false;
    public Vector2 previousCell;
    public int x, y;
    public GameObject northWall, southWall, eastWall, westWall, floor, ceiling;
	public GameObject[] pillars = new GameObject[3];
    public bool hasNorthWall = true;
    public bool hasSouthWall = true;
    public bool hasEastWall = true;
    public bool hasWestWall = true;

    public SpecialCell specialCell;
    public bool hasSpecialCell {
        get {
            return specialCell != null;
        }
    }

    public void CreateSpecialCell () {
        specialCell = new SpecialCell (this);
    }
}

public class SpecialCell {
    public CellType cellType;
    public MazeCell mazeCell;
    public float distanceFromStart;
    public GameObject prefabToSpawn;
    public GameObject prefabInstance;
    public int specialIndex;

    public SpecialCell (MazeCell mazeCell) {
        this.mazeCell = mazeCell;
    }

    public void SpawnPrefab () {
        prefabInstance = GameObject.Instantiate (prefabToSpawn, mazeCell.position + Vector3.up * 3f, Quaternion.identity);
    }

    public void RequestPath (Vector3 startPosition) {
        PathRequestManager.RequestPath (new PathRequest (mazeCell.position, startPosition, OnPathFound));
    }

    public void OnPathFound (Vector3 [] waypoints, bool pathSuccessful) {
        if (pathSuccessful) {
            for (int i = 0; i < waypoints.Length; i++) {
                if (i > 0) {
                    distanceFromStart += Vector3.Distance (waypoints [i], waypoints [i - 1]);
                }
            }
            LevelManager.instance.OnDistanceFound(distanceFromStart, specialIndex);
        } else {
            Debug.Log ("Path Unsuccsessful");
        }
    }
}