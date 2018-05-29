using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 0 1 5
// A flashy projectile shoots from wand, then disappears
public class Spell_FlashyMissile : Spell {

	public GameObject projectilePrefab;
	public int projectileSpeed;
	public float projectileTime;

	public override void Cast () {
		GameObject missile = (GameObject)Instantiate (projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
		Rigidbody body = missile.GetComponent<Rigidbody> ();

		body.velocity = projectileSpawn.transform.forward * projectileSpeed;
		Destroy (missile, projectileTime);

	}
}
