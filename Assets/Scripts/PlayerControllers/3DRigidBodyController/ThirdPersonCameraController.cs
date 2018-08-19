using UnityEngine;
using System.Collections;

public class ThirdPersonCameraController : MonoBehaviour {
	private const float Y_ANGLE_MIN = 0f;
	private const float Y_ANGLE_MAX = 50f;

	public Transform player;
	public Transform camTransform;

	private Camera cam;

	private float maxDistance = 5f;
	private float distance = 0f;
	private float currentX = 0f;
	private float currentY = 0f;
	public float sensitivityX = 0f;
	public float sensitivityY = 0f;

	private void Start () {
		camTransform = transform;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void GetPlayerTransform (Transform _player) {
		player = _player;
	}

	private void Update () {
		if (player != null) {
			RaycastHit hit;

			if (Input.GetKeyDown (KeyCode.Escape)) {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}

			currentX += Input.GetAxis ("Mouse X") * sensitivityX;
			currentY += -Input.GetAxis ("Mouse Y") * sensitivityY;

			currentY = Mathf.Clamp (currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

			//Debug.DrawRay (player.position, transform.TransformDirection (Vector3.back) * maxDistance, Color.red);
			if (Physics.Raycast (player.position, transform.TransformDirection (Vector3.back), out hit, maxDistance)) {
				distance = hit.distance;
			} else
				distance = maxDistance;
		}
	}

	private void LateUpdate () {
		if (player != null) {
			Vector3 dir = new Vector3 (0, 0, -distance);
			Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
			camTransform.position = player.position + rotation * dir;

			camTransform.LookAt (player.position);
			player.rotation = Quaternion.Euler (0, currentX, 0);
		}
	}
}