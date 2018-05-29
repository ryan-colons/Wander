using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

	public static float[,] GenerateNoiseMap (int size, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
		float[,] noiseMap = new float[size, size];

		if (scale <= 0) {
			scale = 0.001f;
		}

		// offset with seed-based randomness
		System.Random rng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;

		for (int i = 0; i < octaves; i++) {
			float offsetX = rng.Next (-100000, 100000) + offset.x;
			float offsetY = rng.Next (-100000, 100000) - offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}


		//float maxNoiseHeight = float.MinValue;
		//float minNoiseHeight = float.MaxValue;

		float halfWidth = size / 2f;
		float halfHeight = size / 2f;

		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x-halfWidth + octaveOffsets [i].x) / scale * frequency;
					float sampleY = (y-halfHeight + octaveOffsets [i].y) / scale * frequency;
					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;
					amplitude *= persistance;
					frequency *= lacunarity;
				}
				/*
				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}
				*/
				noiseMap [x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				//noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);

				float normalisedHeight = (noiseMap[x,y] + 1) / (2f * maxPossibleHeight);
				//noiseMap [x, y] = Mathf.Clamp(normalisedHeight, 0, int.MaxValue);

				noiseMap [x, y] = Mathf.Lerp (0f, maxPossibleHeight, normalisedHeight);
			}
		}

		return noiseMap;
	}
		
}
