using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (CaptureManager))]
public class CaptureManagerEditor : Editor {

    public override void OnInspectorGUI () {
        CaptureManager captureManager = (CaptureManager) target;

        DrawDefaultInspector ();

        if (!Application.isPlaying) {
            if (GUILayout.Button ("Clear Capture folder")) {
                captureManager.ClearCaptureFolder ();
            }
        }
    }
}
