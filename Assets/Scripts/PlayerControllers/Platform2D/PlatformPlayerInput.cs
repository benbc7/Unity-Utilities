using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlatformPlayer))]
public class PlatformPlayerInput : MonoBehaviour {

	private PlatformPlayer player;

	// Use this for initialization
	private void Start () {
		player = GetComponent<PlatformPlayer> ();
	}

	// Update is called once per frame
	private void Update () {
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);

		if (Input.GetButtonDown ("Jump")) {
			player.OnJumpInputDown ();
		}
		if (Input.GetButtonUp ("Jump")) {
			player.OnJumpInputUp ();
		}
	}
}