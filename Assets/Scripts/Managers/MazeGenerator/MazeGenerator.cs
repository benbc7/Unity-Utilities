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

[RequireComponent (typeof (LevelManager))]
public class MazeGenerator : MonoBehaviour {

	public enum AlgorithmType {
		None, HuntAndKill
	}

	public AlgorithmType algorithmType;
	public int sizeX;
	public int sizeY;
	public float scale;
	public bool useRandomSeed;

	[SerializeField]
	public string seed;
	public bool generateCeiling;
	public bool autoUpdate;
	public bool drawGizmos;

	private LevelManager levelManager;
	private System.Random prng;
	private float spacing;

	[HideInInspector, SerializeField]
	public MazeData mazeData;
	private Algorithm algorithm;

	public GameObject pillarPrefab;
	public Material wallMaterial;

	[HideInInspector]
	public bool playing;

	private void Start () {
		levelManager = GetComponent<LevelManager> ();
		if (mazeData.mazeCells == null) {
		}
		playing = true;
		GenerateMaze ();
		CreateMazeObjects ();
	}

	public void GenerateMaze () {
		if (useRandomSeed) {
			seed = Random.Range (0f, 1f).ToString ();
		}
		prng = new System.Random (seed.GetHashCode ());
		if (playing) {
			ClearCreatedMaze ();
		}
		InitializeMaze ();
		SelectAlgorithm ();
		if (algorithm != null) {
			mazeData = algorithm.CreateMaze (prng);
		}
		if (playing) {
			CreateMazeObjects ();
			mazeData.mazeSize = new Vector2 (sizeX, sizeY);
			mazeData.scale = scale;
			mazeData.prng = prng;
			levelManager.GenerateLevel (mazeData);
		}
	}

	private void CreateMazeObjects () {
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {

				Vector3 cellPosition = mazeData.mazeCells [x, y].position;

				mazeData.mazeCells [x, y].floor = CreateCube (cellPosition - Vector3.up * (spacing / 2f), new Vector3 (spacing, scale / 10f, spacing));
				mazeData.mazeCells [x, y].floor.name = "Floor " + x + ", " + y;
				mazeData.mazeCells [x, y].floor.transform.parent = transform;
				mazeData.mazeCells [x, y].floor.layer = 9;

				if (generateCeiling) {
					mazeData.mazeCells [x, y].ceiling = CreateCube (cellPosition + Vector3.up * ((spacing / 2f) + spacing / 10f), new Vector3 (spacing, scale / 10f, spacing));
					mazeData.mazeCells [x, y].ceiling.name = "Ceiling " + x + ", " + y;
					mazeData.mazeCells [x, y].ceiling.transform.parent = mazeData.mazeCells [x, y].floor.transform;
					mazeData.mazeCells [x, y].ceiling.layer = 11;
				}

				CreateWall (x, y, cellPosition);

				CreatePillar (x, y, cellPosition);
			}
		}
	}

	private void CreateWall (int x, int y, Vector3 cellPosition) {
		if (y == 0 && mazeData.mazeCells [x, y].hasWestWall) {
			mazeData.mazeCells [x, y].westWall = CreateCube (cellPosition - Vector3.forward * (spacing / 2f), new Vector3 (spacing, scale, scale / 10f), true);
			mazeData.mazeCells [x, y].westWall.name = "WestWall " + x + ", " + y;
			mazeData.mazeCells [x, y].westWall.transform.parent = mazeData.mazeCells [x, y].floor.transform;
			mazeData.mazeCells [x, y].westWall.layer = 11;
		}

		if (mazeData.mazeCells [x, y].hasEastWall) {
			mazeData.mazeCells [x, y].eastWall = CreateCube (cellPosition + Vector3.forward * (spacing / 2f), new Vector3 (spacing, scale, scale / 10f), true);
			mazeData.mazeCells [x, y].eastWall.name = "EastWall " + x + ", " + y;
			mazeData.mazeCells [x, y].eastWall.transform.parent = mazeData.mazeCells [x, y].floor.transform;
			mazeData.mazeCells [x, y].eastWall.layer = 11;
		}

		if (x == 0 && mazeData.mazeCells [x, y].hasNorthWall) {
			mazeData.mazeCells [x, y].northWall = CreateCube (cellPosition - Vector3.right * (spacing / 2f), new Vector3 (scale / 10f, scale, spacing), true);
			mazeData.mazeCells [x, y].northWall.name = "NorthWall " + x + ", " + y;
			mazeData.mazeCells [x, y].northWall.transform.parent = mazeData.mazeCells [x, y].floor.transform;
			mazeData.mazeCells [x, y].northWall.layer = 11;
		}

		if (mazeData.mazeCells [x, y].hasSouthWall) {
			mazeData.mazeCells [x, y].southWall = CreateCube (cellPosition + Vector3.right * (spacing / 2f), new Vector3 (scale / 10f, scale, spacing), true);
			mazeData.mazeCells [x, y].southWall.name = "SouthWall " + x + ", " + y;
			mazeData.mazeCells [x, y].southWall.transform.parent = mazeData.mazeCells [x, y].floor.transform;
			mazeData.mazeCells [x, y].southWall.layer = 11;
		}
	}

	private void CreatePillar (int x, int y, Vector3 cellPosition) {
		mazeData.mazeCells [x, y].pillars [0] = Instantiate (pillarPrefab, cellPosition + Vector3.right * (spacing / 2f) + Vector3.forward * (spacing / 2f), Quaternion.identity);
		mazeData.mazeCells [x, y].pillars [0].transform.localScale = Vector3.one * scale;
		mazeData.mazeCells [x, y].pillars [0].name = "Pillar " + x + ", " + y + "_0";
		mazeData.mazeCells [x, y].pillars [0].layer = 11;
		mazeData.mazeCells [x, y].pillars [0].transform.parent = mazeData.mazeCells [x, y].floor.transform;

		if (x == 0) {
			mazeData.mazeCells [x, y].pillars [1] = Instantiate (pillarPrefab, cellPosition - Vector3.right * (spacing / 2f) + Vector3.forward * (spacing / 2f), Quaternion.identity);
			mazeData.mazeCells [x, y].pillars [1].transform.localScale = Vector3.one * scale;
			mazeData.mazeCells [x, y].pillars [1].name = "Pillar " + x + ", " + y + "_1";
			mazeData.mazeCells [x, y].pillars [1].layer = 11;
			mazeData.mazeCells [x, y].pillars [1].transform.parent = mazeData.mazeCells [x, y].floor.transform;
		}

		if (y == 0) {
			mazeData.mazeCells [x, y].pillars [2] = Instantiate (pillarPrefab, cellPosition + Vector3.right * (spacing / 2f) - Vector3.forward * (spacing / 2f), Quaternion.identity);
			mazeData.mazeCells [x, y].pillars [2].transform.localScale = Vector3.one * scale;
			mazeData.mazeCells [x, y].pillars [2].name = "Pillar " + x + ", " + y + "_2";
			mazeData.mazeCells [x, y].pillars [2].layer = 11;
			mazeData.mazeCells [x, y].pillars [2].transform.parent = mazeData.mazeCells [x, y].floor.transform;
		}
	}

	private void SelectAlgorithm () {
		switch (algorithmType) {
			case AlgorithmType.HuntAndKill: {
				algorithm = new HuntAndKillAlgorithm (mazeData);
			}
			break;
			case AlgorithmType.None: {
				algorithm = null;
			}
			break;
		}
	}

	private GameObject CreateCube (Vector3 position, Vector3 scale, bool isWall = false) {
		GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		cube.transform.position = position;
		cube.transform.localScale = scale;
		cube.GetComponent<MeshRenderer> ().material = wallMaterial;
		return cube;
	}

	private void InitializeMaze () {
		mazeData = new MazeData ();
		mazeData.mazeCells = new MazeCell [sizeX, sizeY];
		mazeData.specialCells = new List<SpecialCell> ();
		spacing = scale - (scale / 10f);

		Vector3 worldBottomLeft = transform.position - Vector3.right * (float) sizeX * spacing / 2f - Vector3.forward * (float) sizeY * spacing / 2f;
		worldBottomLeft += Vector3.right * spacing / 2f + Vector3.forward * spacing / 2f;

		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * spacing) + Vector3.forward * (y * spacing);
				mazeData.mazeCells [x, y] = new MazeCell ();
				mazeData.mazeCells [x, y].x = x;
				mazeData.mazeCells [x, y].y = y;
				mazeData.mazeCells [x, y].position = worldPoint;
			}
		}
	}

	private void ClearCreatedMaze () {
		if (mazeData.mazeCells != null) {
			for (int x = 0; x < sizeX; x++) {
				for (int y = 0; y < sizeY; y++) {
					Destroy (mazeData.mazeCells [x, y].floor);
				}
			}
		}
		mazeData = null;
	}

	private void OnDrawGizmos () {
		if (drawGizmos && !playing && mazeData.mazeCells != null) {
			Gizmos.color = Color.black;
			for (int x = 0; x < sizeX; x++) {
				for (int y = 0; y < sizeY; y++) {
					if (mazeData.mazeCells [x, y].hasWestWall) {
						Gizmos.DrawCube (mazeData.mazeCells [x, y].position - Vector3.forward * (spacing / 2f), new Vector3 (scale, scale, scale / 10f));
					}
					if (mazeData.mazeCells [x, y].hasEastWall) {
						Gizmos.DrawCube (mazeData.mazeCells [x, y].position + Vector3.forward * (spacing / 2f), new Vector3 (scale, scale, scale / 10f));
					}
					if (mazeData.mazeCells [x, y].hasNorthWall) {
						Gizmos.DrawCube (mazeData.mazeCells [x, y].position - Vector3.right * (spacing / 2f), new Vector3 (scale / 10f, scale, scale));
					}
					if (mazeData.mazeCells [x, y].hasSouthWall) {
						Gizmos.DrawCube (mazeData.mazeCells [x, y].position + Vector3.right * (spacing / 2f), new Vector3 (scale / 10f, scale, scale));
					}
				}
			}
		}
	}

	private void OnValidate () {
		if (sizeX < 3) {
			sizeX = 3;
		}
		if (sizeY < 3) {
			sizeY = 3;
		}
		if (scale < 1) {
			scale = 1;
		}
	}
}

[System.Serializable]
public class MazeData {
	public MazeCell [,] mazeCells;
	public List<SpecialCell> specialCells;
	public System.Random prng;
	public Vector2 mazeSize;
	public float scale;
}