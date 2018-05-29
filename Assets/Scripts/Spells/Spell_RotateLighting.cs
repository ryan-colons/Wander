using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_RotateLighting : Spell {

	public GameObject directionalLight;
	public float spinTime;
	public float spinSpeed;
	private bool spin = false;

	private void Update () {
		if (spin) {
			directionalLight.transform.Rotate (Vector3.right * Time.deltaTime * spinSpeed);
		}
	}

	public override void Cast () {
		StartCoroutine (BeginSpin());
	}

	public IEnumerator BeginSpin () {
		spin = true;
		yield return new WaitForSeconds (spinTime);
		spin = false;
	}
}
