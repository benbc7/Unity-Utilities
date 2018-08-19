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

public class HuntAndKillAlgorithm : Algorithm {

	private int currentX = 0;
	private int currentY = 0;

	private bool courseComplete = false;

	public HuntAndKillAlgorithm (MazeData mazeData) : base (mazeData) {

	}

	public override MazeData CreateMaze (System.Random prng) {
		this.prng = prng;
		HuntAndKill ();
		return mazeData;
	}

	private void HuntAndKill () {
		mazeData.mazeCells [currentX, currentY].visited = true;
		mazeData.mazeCells [currentX, currentY].CreateSpecialCell ();
		mazeData.mazeCells [currentX, currentY].specialCell.cellType = CellType.Start;
        mazeData.mazeCells [currentX, currentY].specialCell.specialIndex = 0;
        mazeData.specialCells.Add (mazeData.mazeCells [currentX, currentY].specialCell);
		mazeData.mazeCells [currentX, currentY].hasNorthWall = false;

		while (!courseComplete) {
			Kill ();
			Hunt ();
		}
	}

	#region Functions

	private void Kill () {
		while (RouteStillAvailable (currentX, currentY)) {
			int direction = prng.Next (1, 5);

			if (direction == 1 && CellIsAvailable (currentX - 1, currentY)) {
				//North
				mazeData.mazeCells [currentX, currentY].hasNorthWall = false;
				mazeData.mazeCells [currentX - 1, currentY].hasSouthWall = false;
				currentX--;
			} else if (direction == 2 && CellIsAvailable (currentX + 1, currentY)) {
				//South
				mazeData.mazeCells [currentX, currentY].hasSouthWall = false;
				mazeData.mazeCells [currentX + 1, currentY].hasNorthWall = false;
				currentX++;
			} else if (direction == 3 && CellIsAvailable (currentX, currentY + 1)) {
				//East
				mazeData.mazeCells [currentX, currentY].hasEastWall = false;
				mazeData.mazeCells [currentX, currentY + 1].hasWestWall = false;
				currentY++;
			} else if (direction == 4 && CellIsAvailable (currentX, currentY - 1)) {
				//West
				mazeData.mazeCells [currentX, currentY].hasWestWall = false;
				mazeData.mazeCells [currentX, currentY - 1].hasEastWall = false;
				currentY--;
			}

			mazeData.mazeCells [currentX, currentY].visited = true;
		}
	}

	private void Hunt () {
		courseComplete = true;

		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				if (!mazeData.mazeCells [x, y].visited && CellHasAnAdjacentVisitedCell (x, y)) {
					courseComplete = false;
					currentX = x;
					currentY = y;
					RemoveAdjacentWall (currentX, currentY);
					mazeData.mazeCells [currentX, currentY].visited = true;
					return;
				}
			}
		}
	}

	private bool RouteStillAvailable (int x, int y) {
		int availableRoutes = 0;

		if (x > 0 && !mazeData.mazeCells [x - 1, y].visited) {
			availableRoutes++;
		}
		if (x < sizeX - 1 && !mazeData.mazeCells [x + 1, y].visited) {
			availableRoutes++;
		}
		if (y > 0 && !mazeData.mazeCells[x, y - 1].visited) {
			availableRoutes++;
		}
		if (y < sizeY - 1 && !mazeData.mazeCells [x, y + 1].visited) {
			availableRoutes++;
		}

		if (availableRoutes == 0 && !mazeData.specialCells.Contains (mazeData.mazeCells[x, y].specialCell)) {
			mazeData.mazeCells [x, y].CreateSpecialCell ();
            mazeData.mazeCells [x, y].specialCell.specialIndex = mazeData.specialCells.Count;
			mazeData.specialCells.Add (mazeData.mazeCells [x, y].specialCell);
		}

		return availableRoutes > 0;
	}

	private bool CellIsAvailable (int x, int y) {
		if (x >= 0 && x < sizeX && y >= 0 && y < sizeY && !mazeData.mazeCells [x, y].visited) {
			return true;
		} else {
			return false;
		}
	}

	private bool CellHasAnAdjacentVisitedCell (int x, int y) {
		int visitedCells = 0;

		if (x > 0 && mazeData.mazeCells [x - 1, y].visited) {
			visitedCells++;
		}
		if (x < (sizeX - 2) && mazeData.mazeCells [x + 1, y].visited) {
			visitedCells++;
		}
		if (y > 0 && mazeData.mazeCells [x, y - 1].visited) {
			visitedCells++;
		}
		if (y < (sizeY - 2) && mazeData.mazeCells [x, y + 1].visited) {
			visitedCells++;
		}

		return visitedCells > 0;
	}

	private void RemoveAdjacentWall (int x, int y) {
		bool wallRemoved = false;

		while (!wallRemoved) {
			int direction = prng.Next (1, 5);

			if (direction == 1 && x > 0 && mazeData.mazeCells [x - 1, y].visited) {
				mazeData.mazeCells [x, y].hasNorthWall = false;
				mazeData.mazeCells [x - 1, y].hasSouthWall = false;
				wallRemoved = true;
			} else if (direction == 2 && x < (sizeX - 2) && mazeData.mazeCells [x + 1, y].visited) {
				mazeData.mazeCells [x, y].hasSouthWall = false;
				mazeData.mazeCells [x + 1, y].hasNorthWall = false;
				wallRemoved = true;
			} else if (direction == 3 && y > 0 && mazeData.mazeCells [x, y - 1].visited) {
				mazeData.mazeCells [x, y].hasWestWall = false;
				mazeData.mazeCells [x, y - 1].hasEastWall = false;
				wallRemoved = true;
			} else if (direction == 4 && y < (sizeY - 2) && mazeData.mazeCells [x, y + 1].visited) {
				mazeData.mazeCells [x, y].hasEastWall = false;
				mazeData.mazeCells [x, y + 1].hasWestWall = false;
				wallRemoved = true;
			}
		}
	}

	#endregion
}