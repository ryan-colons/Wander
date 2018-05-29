using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_BiomeShift : Spell {

	private MapGenerator mapGenerator;
	private InfiniteTerrain infiniteTerrain;
	private bool shift = false;
	public float shiftSpeed;
	public float shiftTime;

	public void Start () {
		mapGenerator = FindObjectOfType<MapGenerator> ();
		infiniteTerrain = FindObjectOfType<InfiniteTerrain> ();
		shift = false;
	}

	private void Update () {
		if (shift) {
			mapGenerator.splat.offset.x += shiftSpeed;
			// redraw all maps somehow :O
			// performance on this is probably terrible lol
			infiniteTerrain.RedrawAllTextures();

		}
	}

	public override void Cast () {
		StartCoroutine (BeginShift ());
	}

	public IEnumerator BeginShift () {
		shift = true;
		yield return new WaitForSeconds (shiftTime);
		shift = false;
	}

}
