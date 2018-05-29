using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Disintegrate : Spell {

	public GameObject projectilePrefab;
	public int projectileSpeed;
	public float projectileTime;

	public override void Cast () {
		GameObject missile = (GameObject)Instantiate (projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
		Rigidbody body = missile.GetComponent<Rigidbody> ();

		body.velocity = projectileSpawn.transform.forward * projectileSpeed;
		Destroy (missile, projectileTime);

		missile.AddComponent<DisintegrationMissile> ();
	}
}
	
public class DisintegrationMissile : MonoBehaviour {
	public void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag.Equals (Spell.spellTag)) {
			other.gameObject.AddComponent<DisintegrateEffect> ();
			Destroy (gameObject);
		}

	}
}

public class DisintegrateEffect : MonoBehaviour {
	private Renderer renderer;
	private MaterialPropertyBlock propBlock;
	private float disintegration;
	public void Start () {
		renderer = GetComponent<Renderer>();
		Texture texture = renderer.material.mainTexture;
		//renderer.material = new Material (texture);

		renderer.material.shader = Shader.Find("Custom/FireEffectShader");
		propBlock = new MaterialPropertyBlock ();
		disintegration = 0;
	}
	public void Update () {
		renderer.GetPropertyBlock (propBlock);
		propBlock.SetFloat ("_DissolveAmount", disintegration);
		disintegration += 0.05f;
		renderer.SetPropertyBlock (propBlock);
	}
}


