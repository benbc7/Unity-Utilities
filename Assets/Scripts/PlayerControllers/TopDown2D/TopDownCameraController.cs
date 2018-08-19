/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Camera))]
public class TopDownCameraController : MonoBehaviour {

	public Collider2D targetCollider2D;
	public float lookAheadDistance = 2f;
	public float lookSmoothTime = 0.5f;
	public Vector2 focusAreaSize = new Vector2 (5f, 5f);

	private Camera viewCamera;
	private FocusArea focusArea;
	private Vector3 mousePosition;
	private Vector2 lookAheadDirection;
	private Vector2 currentLookAhead;
	private Vector2 targetLookAhead;
	private float smoothLookVelocityX;
	private float smoothLookVelocityY;

	public void SetUpCamera (Collider2D collider2D) {
		viewCamera = GetComponent<Camera> ();
		viewCamera.orthographicSize = 10;
		targetCollider2D = collider2D;
		focusArea = new FocusArea (targetCollider2D.bounds, focusAreaSize);
	}

	private void LateUpdate () {
		if (targetCollider2D != null) {
			focusArea.Update (targetCollider2D.bounds);
			Vector2 focusPosition = focusArea.center;

			mousePosition = viewCamera.ScreenToWorldPoint (Input.mousePosition);
			targetLookAhead = (mousePosition - targetCollider2D.bounds.center).normalized * lookAheadDistance;
			currentLookAhead.x = Mathf.SmoothDamp (currentLookAhead.x, targetLookAhead.x, ref smoothLookVelocityX, lookSmoothTime);
			currentLookAhead.y = Mathf.SmoothDamp (currentLookAhead.y, targetLookAhead.y, ref smoothLookVelocityY, lookSmoothTime);

			focusPosition += currentLookAhead;
			transform.position = (Vector3) focusPosition + Vector3.forward * -10;
		}
	}

	private void OnDrawGizmos () {
		Gizmos.color = new Color (1, 0, 0, 0.5f);
		Gizmos.DrawCube (focusArea.center, focusAreaSize);
	}

	private struct FocusArea {
		public Vector2 center;
		public Vector2 velocity;
		private float left, right, top, bottom;

		public FocusArea (Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.center.y - size.y / 2;
			top = targetBounds.center.y + size.y / 2;

			velocity = Vector2.zero;
			center = new Vector2 ((left + right) / 2, (top + bottom) / 2);
		}

		public void Update (Bounds targetBounds) {
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}

			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}

			top += shiftY;
			bottom += shiftY;

			center = new Vector2 ((left + right) / 2, (top + bottom) / 2);
			velocity = new Vector2 (shiftX, shiftY);
		}
	}
}