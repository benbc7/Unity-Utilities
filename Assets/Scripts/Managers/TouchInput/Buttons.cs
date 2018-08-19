/*
* Copyright (c) Ben Cutler
* Tetricom Studios
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType {
	Move, Brake, Primary, Secondary, Pause, Resume, Restart, Quit
}

public class Buttons : MonoBehaviour {

	public ButtonType buttonType;

	private Image image;
	private BoxCollider boxCollider;

	private bool buttonPressed;

	private bool paused;

	private void Start () {
		image = GetComponent<Image> ();
		SetUpCollider ();
	}

	private void OnTouchDown () {
		switch (buttonType) {
			case ButtonType.Move: {
				buttonPressed = true;
			}
			break;
			case ButtonType.Secondary: {
			}
			break;
			case ButtonType.Pause: {
				buttonPressed = true;
			}
			break;
			case ButtonType.Resume: {
			}
			break;
			case ButtonType.Restart: {
			}
			break;
			case ButtonType.Quit: {
			}
			break;
		}
	}

	private void OnTouchUp (Vector3 touchPosition) {
		switch (buttonType) {
			case ButtonType.Brake: {
			}
			break;
			case ButtonType.Pause: {
				if (buttonPressed) {
				} else {
				}
			}
			break;
		}
	}

	private void OnTouchStay (Vector3 touchPosition) {
		switch (buttonType) {
			case ButtonType.Move: {
				if (buttonPressed) {
				}
			}
			break;
			case ButtonType.Brake: {
			}
			break;
			case ButtonType.Primary: {
			}
			break;
			case ButtonType.Pause: {
				if (!buttonPressed) {
				}
			}
			break;
		}
	}

	private void OnTouchExit () {
		switch (buttonType) {
			case ButtonType.Pause: {
				buttonPressed = false;
			}
			break;
		}
	}

	private void SetUpCollider () {
		Vector3 colliderSize = new Vector3 (image.rectTransform.rect.width, image.rectTransform.rect.height, 1);
		BoxCollider collider = gameObject.AddComponent<BoxCollider> ();
		collider.size = colliderSize;
	}
}