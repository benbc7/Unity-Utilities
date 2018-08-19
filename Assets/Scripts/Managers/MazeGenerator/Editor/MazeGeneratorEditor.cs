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
using UnityEditor;

[CustomEditor (typeof (MazeGenerator))]
public class MazeGeneratorEditor : Editor {

	public override void OnInspectorGUI () {
		MazeGenerator mazeGen = (MazeGenerator) target;

		if (DrawDefaultInspector ()) {
			if (mazeGen.autoUpdate) {
				mazeGen.GenerateMaze ();
			}
		}

		if (GUILayout.Button ("Generate")) {
			mazeGen.GenerateMaze ();
			if (!mazeGen.playing) {
				SceneView.RepaintAll ();
			}
		}
	}
}