/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Living : MonoBehaviour, IDamagable {

	[Header ("Living Entity")]
	public float startingHealth;

	[Tooltip ("If the entity has a healthbar")]
	public Image healthBar;

	protected float health;
	private bool _dead = false;

	public bool dead {
		get {
			return _dead;
		}
		protected set {
			_dead = value;
		}
	}

	protected virtual void Start () {
		SetDefaults ();
	}

	public virtual void SetDefaults () {
		health = startingHealth;
		dead = false;
	}

	public void TakeDamage (float damage) {
		if (!dead) {
			health -= damage;
			if (healthBar != null) {
				healthBar.fillAmount = health / startingHealth;
			}

			if (health <= 0) {
				Die ();
			}
		}
	}

	protected virtual void Die () {
		dead = true;
	}
}