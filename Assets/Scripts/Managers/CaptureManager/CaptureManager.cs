using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CaptureManager : MonoBehaviour {

	public static CaptureManager instance;

	public int captureWidth = 1920;
	public int captureHeight = 1080;

	private string folder;

	private Rect rect;
	private RenderTexture renderTexture;
	private Texture2D screenShot;
	private string mask;
	private int counter = 0; // image #

	private string slash;

	private void Awake () {
		instance = GetComponent<CaptureManager> ();
		SetupCapturePath ();
	}

	public string CaptureScreenshot () {
		if (renderTexture == null) {
			rect = new Rect (0, 0, captureWidth, captureHeight);
			renderTexture = new RenderTexture (captureWidth, captureHeight, 24);
			screenShot = new Texture2D (captureWidth, captureHeight, TextureFormat.RGB24, false);
		}

		Camera viewCamera = this.GetComponent<Camera> (); // NOTE: added because there was no reference to camera in original script; must add this script to Camera
		viewCamera.targetTexture = renderTexture;
		viewCamera.Render ();

		// read pixels will read from the currently active render texture so make our offscreen
		// render texture active and then read the pixels
		RenderTexture.active = renderTexture;
		screenShot.ReadPixels (rect, 0, 0);

		// reset active camera texture and render texture
		viewCamera.targetTexture = null;
		RenderTexture.active = null;

		// get our unique filename
		string filename = UniqueFilename ();

		// pull in our file header/data bytes for the specified image format (has to be done from main thread)
		byte [] fileHeader = null;
		byte [] fileData = null;

		fileData = screenShot.EncodeToPNG ();

		// create new thread to save the image to file (only operation that can be done in background)
		new System.Threading.Thread (() => {

			// create file and write optional header with image bytes
			var f = System.IO.File.Create (filename);
			if (fileHeader != null)
				f.Write (fileHeader, 0, fileHeader.Length);
			f.Write (fileData, 0, fileData.Length);
			f.Close ();

			//Debug.Log (string.Format ("Wrote screenshot {0} of size {1}", filename, fileData.Length));
		}).Start ();

		Destroy (renderTexture);
		renderTexture = null;
		screenShot = null;

		return filename;
	}

	public void CaptureScreenshot (int captureWidth, int captureHeight) {
		this.captureWidth = captureWidth;
		this.captureHeight = captureHeight;
		CaptureScreenshot ();
	}

	private void SetupCapturePath () {

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		slash = "\\";
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		slash = "/";
#endif

		folder = Application.persistentDataPath;

		folder += slash + "captures";

		System.IO.Directory.CreateDirectory (folder);

		mask = string.Format ("capture_*.png");
		counter = Directory.GetFiles (folder, mask, SearchOption.TopDirectoryOnly).Length;
	}

	public void ClearCaptureFolder () {
		SetupCapturePath ();

		string [] files = Directory.GetFiles (folder);

		for (int i = 0; i < files.Length; i++) {
			File.Delete (files [i]);
		}

		if (File.Exists (folder + "CaptureData.json")) {
			File.Delete (folder + "CaptureData.json");
			counter++;
		}
		Debug.Log (counter + " Files deleted from Captures folder");

		counter = 0;
	}

	private string UniqueFilename () {
		var filename = string.Format ("{0}{1}capture_{2}.png", folder, slash, counter);

		++counter;

		return filename;
	}
}