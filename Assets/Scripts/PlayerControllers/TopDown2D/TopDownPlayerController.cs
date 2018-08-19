/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class TopDownPlayerController : MonoBehaviour {

	private Vector2 velocity = Vector2.zero;
	private Rigidbody2D rb;

	private void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}

	public void Move (Vector2 newVelocity) {
		velocity = newVelocity;
	}

	private void FixedUpdate () {
		if (velocity != Vector2.zero) {
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}
	}
}