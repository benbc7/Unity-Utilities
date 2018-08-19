using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotExample : MonoBehaviour {

    private List<CaptureData> captureDataList;

    private void Start () {
        captureDataList = SaveManager.instance.GetCaptureData ();
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.Space)) {
            captureDataList.Add (new CaptureData (CaptureManager.instance.CaptureScreenshot ()));
        }

        if (Input.GetKeyDown (KeyCode.S)) {
            SaveManager.instance.SaveCaptureData (captureDataList);
        }
    }
}
