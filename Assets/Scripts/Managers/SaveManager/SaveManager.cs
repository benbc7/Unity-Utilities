using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveManager : MonoBehaviour {

	public static SaveManager instance;

	private List<CaptureData> captureDataList;
	private string jsonPath;
	private string jsonString;
	private string slash;

	private void Awake () {
		instance = GetComponent<SaveManager> ();
		AwakeJSONSetup ();
	}

	#region Capture Data Functions

	private void AwakeJSONSetup () {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		slash = "\\";
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		slash = "/";
#endif

		jsonPath = Application.persistentDataPath + slash + "captures" + slash + "CaptureData.json";
		LoadJSONCaptureData ();
	}

	private void SaveJSONCaptureData () {
		JSONSaveData tempSaveData = new JSONSaveData (captureDataList);
		jsonString = JsonUtility.ToJson (tempSaveData);
		File.WriteAllText (jsonPath, jsonString);
	}

	private void LoadJSONCaptureData () {
		if (File.Exists (jsonPath)) {
			jsonString = File.ReadAllText (jsonPath);
			JSONSaveData tempSaveData = JsonUtility.FromJson<JSONSaveData> (jsonString);
			captureDataList = tempSaveData.captureDataList;
		} else {
			captureDataList = new List<CaptureData> ();
		}
	}

	public List<CaptureData> GetCaptureData () {
		return captureDataList;
	}

	public void SaveCaptureData (List<CaptureData> captureData) {
		captureDataList = captureData;
		SaveJSONCaptureData ();
	}

	#endregion Capture Data Functions

	[Serializable]
	public class JSONSaveData {
		public List<CaptureData> captureDataList;

		public JSONSaveData (List<CaptureData> captureData) {
			captureDataList = captureData;
		}
	}
}

[Serializable]
public class CaptureData {
	public string imagePath;
	public string description;

	public CaptureData (string imagePath) {
		this.imagePath = imagePath;
	}
}