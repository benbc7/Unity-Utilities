/*
* Copyright (c) Ben Cutler
* Tetricom Studios
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchInput : MonoBehaviour {

	#region Variables

	public LayerMask touchInputMask;
	public List<Image> buttons;

	private List<GameObject> touchList = new List<GameObject> ();
	private GameObject[] touchesOld;

	private RaycastHit hit;

	private Camera cam;

	#endregion

	void Start () {
		cam = GetComponent<Camera> ();
	}

	void Update () {

#if UNITY_EDITOR

		GetEditorInput ();

#endif

		GetTouchInput ();

	}

	#region Functions

	void GetEditorInput () {
		if (Input.GetMouseButton (0) || Input.GetMouseButtonDown (0) || Input.GetMouseButtonUp (0)) {

			touchesOld = new GameObject[touchList.Count];
			touchList.CopyTo (touchesOld);
			touchList.Clear ();

			Ray ray = cam.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit, touchInputMask)) {

				GameObject recipient = hit.transform.gameObject;
				touchList.Add (recipient);

				if (Input.GetMouseButtonDown (0)) {
					recipient.SendMessage ("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
				}
				if (Input.GetMouseButtonUp (0)) {
					recipient.SendMessage ("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
				}
				if (Input.GetMouseButton (0)) {
					recipient.SendMessage ("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}

			foreach (GameObject g in touchesOld) {

				if (!touchList.Contains (g)) {
					g.SendMessage ("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	void GetTouchInput () {
		if (Input.touchCount > 0) {

			touchesOld = new GameObject[touchList.Count];
			touchList.CopyTo (touchesOld);
			touchList.Clear ();

			foreach (Touch touch in Input.touches) {

				Ray ray = cam.ScreenPointToRay (touch.position);

				if (Physics.Raycast (ray, out hit, touchInputMask)) {

					GameObject recipient = hit.transform.gameObject;
					touchList.Add (recipient);

					if (touch.phase == TouchPhase.Began) {
						recipient.SendMessage ("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
					}
					if (touch.phase == TouchPhase.Ended) {
						recipient.SendMessage ("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
					}
					if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
						recipient.SendMessage ("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
					}
					if (touch.phase == TouchPhase.Canceled) {
						recipient.SendMessage ("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
					}
				}
			}

			foreach (GameObject g in touchesOld) {

				if (!touchList.Contains (g)) {
					g.SendMessage ("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	#endregion
}
