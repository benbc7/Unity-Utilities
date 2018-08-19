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
public class Body : MonoBehaviour {

	[HideInInspector]
	public Rigidbody rb;

	public bool attractAtStart = true;

	protected virtual void Awake () {
		rb = GetComponent<Rigidbody> ();
	}

	private void OnEnable () {
		if (Attractor.bodies == null) {
			Attractor.bodies = new List<Body> ();
		}

		if (attractAtStart) {
			Attractor.bodies.Add (this);
		}
	}

	private void OnDisable () {
		Attractor.bodies.Remove (this);
	}
}