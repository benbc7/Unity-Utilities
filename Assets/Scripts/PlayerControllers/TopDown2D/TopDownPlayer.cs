/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (TopDownPlayerController))]
public class TopDownPlayer : MonoBehaviour {

	public float moveSpeed = 6f;
	public float accelerationTime = 0.1f;
	public TopDownCameraController cameraPrefab;

	private Vector2 moveVelocity;
	private TopDownPlayerController controller;
	private TopDownCameraController playerCamera;

	private float velocityXSmoothing;
	private float velocityYSmoothing;

	private void Start () {
		controller = GetComponent<TopDownPlayerController> ();
		GameObject.Find ("Main Camera").SetActive (false);
		playerCamera = Instantiate (cameraPrefab) as TopDownCameraController;
		playerCamera.SetUpCamera (GetComponent<Collider2D> ());
	}

	private void Update () {
		MovementInput ();
	}

	private void MovementInput () {
		Vector2 moveInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		Vector2 targetVelocity = moveInput.normalized * moveSpeed;
		moveVelocity.x = Mathf.SmoothDamp (moveVelocity.x, targetVelocity.x, ref velocityXSmoothing, accelerationTime);
		moveVelocity.y = Mathf.SmoothDamp (moveVelocity.y, targetVelocity.y, ref velocityYSmoothing, accelerationTime);
		controller.Move (moveVelocity);
	}
}