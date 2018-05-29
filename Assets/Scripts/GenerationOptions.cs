using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenerationOptions {

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public float noiseScale;
	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacunarity;
	public int seed;
	public Vector2 offset;

	public void OnValidate () {
		if (lacunarity < 1)
			lacunarity = 1;
		if (octaves < 0)
			octaves = 1;
	}
}

