/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Attractor : MonoBehaviour {

	private const float G = 667.4f;

	public static List<Attractor> attractors;
	public static List<Body> bodies;

	public bool staticAttractor;

	[Tooltip ("0 = infinite")]
	public float maxAttractDistance = 0f;
	private float sqrMaxAttractDistance;

	[HideInInspector]
	public Rigidbody rb;

	private void Awake () {
		rb = GetComponent<Rigidbody> ();
		sqrMaxAttractDistance = maxAttractDistance * maxAttractDistance;
		bodies = new List<Body> ();
	}

	private void FixedUpdate () {
		for (int i = 0; i < attractors.Count; i++) {
			if (attractors [i] != this) {
				Attract (attractors [i].rb);
			}
		}

		for (int i = 0; i < bodies.Count; i++) {
			Attract (bodies [i].rb);
		}
	}

	private void Attract (Rigidbody rbToAttract) {
		Vector3 direction = rb.position - rbToAttract.position;

		if (maxAttractDistance > 0 && direction.sqrMagnitude > sqrMaxAttractDistance) {
			return;
		}

		float distance = direction.magnitude;
		direction.Normalize ();

		if (distance > 0) {
			float forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow (distance, 2);
			Vector3 force = direction * forceMagnitude;
			rbToAttract.AddForce (force);
		}
	}

	private void OnEnable () {
		if (attractors == null) {
			attractors = new List<Attractor> ();
		}

		if (!staticAttractor) {
			attractors.Add (this);
		}
	}

	private void OnDisable () {
		attractors.Remove (this);
	}

	private void OnDrawGizmosSelected () {
		if (maxAttractDistance > 0) {
			Gizmos.color = Color.red;
			SphereCollider collider = GetComponent<SphereCollider> ();
			float colliderRadius = collider.radius * ((transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3);
			Gizmos.DrawWireSphere (transform.position, colliderRadius + maxAttractDistance);
		}
	}

	private void OnValidate () {
		if (maxAttractDistance < 0) {
			maxAttractDistance = 0;
		}
	}
}