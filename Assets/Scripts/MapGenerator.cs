using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public const int mapSize = 121;

	public GenerationOptions terrain;
	public GenerationOptions splat;
	public GenerationOptions objects;
	[SerializeField]
	public List<Biome> biomes;

	public bool autoGen;


	public MapData GenerateMapData (Vector2 centrePoint) {
		float[,] noiseMap = Noise.GenerateNoiseMap (mapSize, terrain.seed, terrain.noiseScale, terrain.octaves, terrain.persistance, terrain.lacunarity, centrePoint + terrain.offset);
		float[,] splatMap = Noise.GenerateNoiseMap(mapSize, splat.seed, splat.noiseScale, splat.octaves, splat.persistance, splat.lacunarity, centrePoint + splat.offset);
		float[,] objectMap = Noise.GenerateNoiseMap(mapSize, objects.seed, objects.noiseScale, objects.octaves, objects.persistance, objects.lacunarity, centrePoint + objects.offset);
		Color[] colorMap = new Color[mapSize * mapSize];

		// use splat and height map to fill colour map
		for (int y = 0; y < mapSize; y++) {
			for (int x = 0; x < mapSize; x++) {
				float terrainNoisePoint = noiseMap[x,y];
				float splatNoisePoint = splatMap [x, y];

				Biome biome = biomes [biomes.Count - 1];
				for (int i = 0; i < biomes.Count; i++) {
					if (splatNoisePoint <= biomes [i].threshold) {
						biome = biomes [i];
						break;
					}

				}

				// set in advance, just in case it doesn't get set in loop (when terrainNoisePoint > 1)
				colorMap [y * mapSize + x] = biome.regions [biome.regions.Count - 1].colour;
				for (int i = 0; i < biome.regions.Count; i++) {
					if (terrainNoisePoint <= biome.regions [i].height) {
						colorMap [y * mapSize + x] = biome.regions [i].colour;
						break;
					}
				}
			}
		}

		return new MapData (noiseMap, colorMap, objectMap);
	}

	public void ConstructMap (Vector2 centrePoint) {
		MapData mapData = GenerateMapData (centrePoint);

		MapDisplay display = FindObjectOfType<MapDisplay> ();
		display.DrawMesh (
			MeshGenerator.GenerateMesh (mapData.heightMap, terrain.meshHeightMultiplier, terrain.meshHeightCurve), 
			TextureGenerator.GenerateTextureFromColorMap (mapData.colorMap, mapSize, mapSize)
		);
	}

	public void Clear () {
		MapDisplay display = FindObjectOfType<MapDisplay> ();
		display.Clear ();
	}
		
}

public struct MapData {
	public readonly float[,] heightMap;
	public readonly Color[] colorMap;
	public readonly float[,] objectMap;

	public MapData (float[,] heightMap, Color[] colorMap, float[,] objectMap) {
		this.heightMap = heightMap;
		this.colorMap = colorMap;
		this.objectMap = objectMap;
	}
}

