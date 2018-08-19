using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlatformPlayerController2D))]
public class PlatformPlayer : MonoBehaviour {

	public float moveSpeed = 6;
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = 0.4f;
	private float accelerationTimeAirborne = 0.2f;
	private float accelerationTimeGrounded = 0.1f;

	public Vector2 wallJumpClimb = new Vector2 (7.5f, 16f);
	public Vector2 wallJumpOff = new Vector2 (8.5f, 7f);
	public Vector2 wallLeap = new Vector2 (18, 17);
	public float wallSlideSpeedMax = 3;
	public float wallStickTime = 0.25f;
	private float timeToWallUnstick;

	private float gravity;
	private float maxJumpVelocity;
	private float minJumpVelocity;
	private Vector3 velocity;
	private float velocityXSmoothing;

	private PlatformPlayerController2D controller;

	private Vector2 directionalInput;
	private bool wallSliding;
	private int wallDirectionX;

	private void Start () {
		controller = GetComponent<PlatformPlayerController2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	private void Update () {
		CalculateVelocity ();
		HandleWallSliding ();

		controller.Move (velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else {
				velocity.y = 0;
			}
		}
	}

	public void SetDirectionalInput (Vector2 input) {
		directionalInput = input;
	}

	public void OnJumpInputDown () {
		if (wallSliding) {
			if (wallDirectionX == directionalInput.x) {
				velocity.x = -wallDirectionX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			} else if (directionalInput.x == 0) {
				velocity.x = -wallDirectionX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			} else {
				velocity.x = -wallDirectionX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
		}
		if (controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { // not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			} else {
				velocity.y = maxJumpVelocity;
			}
		}
	}

	public void OnJumpInputUp () {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}

	private void HandleWallSliding () {
		wallDirectionX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirectionX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				} else {
					timeToWallUnstick = wallStickTime;
				}
			} else {
				timeToWallUnstick = wallStickTime;
			}
		}
	}

	private void CalculateVelocity () {
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
	}
}