using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biome {

	[SerializeField]
	public List<TerrainType> regions = new List<TerrainType> ();
	[SerializeField]
	public float threshold;

	public static int index = 0;

	public Biome (List<TerrainType> newRegions, float threshold) {
		for (int i = 0; i < newRegions.Count; i++) {
			AddRegion (newRegions [i].height, newRegions [i].colour);
		}
		this.threshold = threshold;
	}

	public void incrementIndex (int numBiomes) {
		index++;
		if (index >= numBiomes) {
			index = 0;
		}
	}

	public void Validate () {
		if (regions.Count == 0) {
			AddRegion (1f, Color.magenta);
		}
	}

	// return index of insert point
	public int AddRegion(float height, Color colour) {
		TerrainType newTerrain = new TerrainType(height, colour);
		for (int i = 0; i < regions.Count; i++) {
			if (newTerrain.height < regions [i].height) {
				regions.Insert (i, newTerrain);
				return i;
			}
		}
		regions.Add (newTerrain);
		return regions.Count - 1;
	}

	public void RemoveRegion(int index) {
		if (regions.Count >= 2) {
			regions.RemoveAt (index);
		}
	}

	public Color EvaluateToColour (float t) {
		for (int i = 0; i < regions.Count; i++) {
			if (t <= regions [i].height) {
				return regions [i].colour;
			}
		}
		return Color.black;
	}

	public Texture2D GetHorizTexture (int width) {
		// return texture to draw in property window
		Texture2D texture = new Texture2D (width, 1);
		Color[] colours = new Color[width];
		for (int i = 0; i < width; i++) {
			colours [i] = EvaluateToColour ((float)i / (width - 1));
		}
		texture.SetPixels (colours);
		texture.Apply ();
		return texture;
	}

	public Texture2D GetVertTexture (int height) {
		// return texture to draw in editor window
		Texture2D texture = new Texture2D (1, height);
		Color[] colours = new Color[height];
		for (int i = 0; i < height; i++) {
			colours [i] = EvaluateToColour ((float)i / (height - 1));
		}
		texture.SetPixels (colours);
		texture.Apply ();
		return texture;
	}

	public void SetRegionColour (int index, Color colour) {
		regions [index].colour = colour;
	}

	// return new index of region
	public int SetRegionHeight (int index, float height) {
		Color colour = regions [index].colour;
		RemoveRegion (index);
		return AddRegion (height, colour);
	}
}

[System.Serializable]
public class TerrainType {
	public float height;
	public Color colour;

	public TerrainType (float height, Color colour) {
		this.height = height;
		this.colour = colour;
	}
}
