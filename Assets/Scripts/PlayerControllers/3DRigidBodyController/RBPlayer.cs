/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios LLC
Product:		RigidBody Controller
Date:			10/8/17
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (RBPlayerController))]
public class RBPlayer : MonoBehaviour {

	[Header ("Movement")]
	public float moveSpeed = 5;
	public float runMultiplier = 1.5f;
	public float accelerationTimeGrounded = 0.1f;

	[Header ("Jumping")]
	public float jumpHeight = 4;
	public float timeToJumpApex = 0.4f;
	public float accelerationTimeAirborne = 0.2f;

	private RBPlayerController playerController;
	private Vector3 moveVelocity;
	private float jumpVelocity;
	private float gravity;
	private float currentRunMultiplier = 1f;
	private float velocityXSmoothing;
	private float velocityZSmoothing;

	private void Start () {
		playerController = GetComponent<RBPlayerController> ();
		Camera.main.GetComponent<ThirdPersonCameraController> ().GetPlayerTransform (transform);
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		Physics.gravity = Vector3.up * gravity;
	}

	private void Update () {
		RunInput ();
		MoveInput ();
	}

	private void MoveInput () {
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		currentRunMultiplier = (moveInput.z >= 0) ? currentRunMultiplier : 1;
		Vector3 targetVelocity = transform.TransformDirection (moveInput.normalized) * moveSpeed * currentRunMultiplier;
		moveVelocity.x = Mathf.SmoothDamp (moveVelocity.x, targetVelocity.x, ref velocityXSmoothing, (playerController.grounded) ? accelerationTimeGrounded : accelerationTimeAirborne);
		moveVelocity.z = Mathf.SmoothDamp (moveVelocity.z, targetVelocity.z, ref velocityZSmoothing, (playerController.grounded) ? accelerationTimeGrounded : accelerationTimeAirborne);
		playerController.Move (moveVelocity);
		if (playerController.grounded && Input.GetKeyDown (KeyCode.Space)) {
			playerController.Jump (jumpVelocity);
		}
	}

	private void RunInput () {
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			currentRunMultiplier = runMultiplier;
		}
		if (Input.GetKeyUp (KeyCode.LeftShift)) {
			currentRunMultiplier = 1;
		}
	}
}