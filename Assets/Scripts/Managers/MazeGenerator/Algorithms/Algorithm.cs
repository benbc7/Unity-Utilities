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

public abstract class Algorithm {
	protected MazeData mazeData;
	protected int sizeX, sizeY;
	protected System.Random prng;

	protected Algorithm (MazeData mazeData) : base () {
		this.mazeData = mazeData;
		sizeX = mazeData.mazeCells.GetLength (0);
		sizeY = mazeData.mazeCells.GetLength (1);
	}

	public abstract MazeData CreateMaze (System.Random prng);
}