using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour {

	public GameObject spawnPrefab;

	public int tightness;
	public int minDistBetweenPoints;
	private int dimensions = 3;

	public bool[,] GeneratePoissonGrid (int width, int height, float[,] noiseMap) {
		float cellSize = (float) minDistBetweenPoints / Mathf.Sqrt ((float)dimensions);
		Coord[,] grid = new Coord[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(height / cellSize)];
		List<Coord> processingList = new List<Coord> ();

		Coord initialPoint = new Coord (rng(width), rng(height));
		Coord pointOnGrid = SpaceToPoissonGrid (initialPoint, cellSize);
		grid [pointOnGrid.x, pointOnGrid.y] = initialPoint;
		processingList.Add (initialPoint);

		while (processingList.Count > 0) {
			//get random element
			Coord extantPoint = processingList[rng(processingList.Count)];
			processingList.Remove (extantPoint);

			for (int i = 0; i < tightness; i++) {
				// generate new point around extantPoint
				Coord newCoord = RandomPointInRadius(extantPoint, noiseMap[extantPoint.x, extantPoint.y]);
				// if the new point isn't problematic, add it to processingQueue and grid
				if (!TooCloseToOtherPoints (newCoord, grid, cellSize)) {
					if (!(newCoord.x < 0 || newCoord.x >= width || newCoord.y < 0 || newCoord.y >= height)) {
						Coord newPointOnGrid = SpaceToPoissonGrid(newCoord, cellSize);
						//Debug.Log (newPointOnGrid.x + ":" + newPointOnGrid.y + " ... " + grid.GetLength (0) + ", " + grid.GetLength (1));
						grid[newPointOnGrid.x, newPointOnGrid.y] = newCoord;
						processingList.Add(newCoord);
					}
				}
			}
		}

		bool[,] boolGrid = new bool[width, height];
		for (int x = 0; x < grid.GetLength (0); x++) {
			for (int y = 0; y < grid.GetLength (1); y++) {
				Coord point = grid [x, y];
				if (point != null) {
					boolGrid [point.x, point.y] = true;
				}
			}
		}

		return boolGrid;
	}

	private Coord RandomPointInRadius (Coord coord, float noisePoint) {
		// get random radius between minDist and (2 * minDist)
		float randRadius = minDistBetweenPoints * noisePoint * (Random.Range (0f, 1f) + 1);
		// get random angle
		float randAngle = 2 * Mathf.PI * Random.Range(0f, 1f);
		// generate new coord
		int x = (int)(coord.x + randRadius * Mathf.Cos(randAngle));
		int y = (int)(coord.y + randRadius * Mathf.Sin(randAngle));
		return new Coord (x, y);
	}

	private bool TooCloseToOtherPoints (Coord coord, Coord[,] grid, float cellSize) {
		// get the position on the grid
		Coord gridCoord = SpaceToPoissonGrid (coord, cellSize);
		// check adjacent squares
		for (int x = gridCoord.x - 1; x < gridCoord.x + 1; x++) {
			for (int y = gridCoord.y - 1; y < gridCoord.y + 1; y++) {
				if (x < 0 || x >= grid.GetLength (0) || y < 0 || y >= grid.GetLength (1)) {
					continue;
				}
				Coord cell = grid [x, y];
				if (cell != null) {
					if (coord.Distance (cell) < minDistBetweenPoints) {
						return true;
					}
				}
			}
		}
		return false;
	}

	// return random number from 0 to n-1
	private int rng (int n) {
		return Random.Range (0, n);
	}

	private Coord SpaceToPoissonGrid (Coord coord, float cellSize) {
		int gridX = (int)(coord.x / cellSize);
		int gridY = (int)(coord.y / cellSize);
		return new Coord (gridX, gridY);
	}

	public List<GameObject> GenerateObjects (MeshData meshData, float[,] objectMap, Vector2 offset) {
		List<GameObject> objList = new List<GameObject> ();

		bool[,] poissonDistribution = GeneratePoissonGrid (MapGenerator.mapSize, MapGenerator.mapSize, objectMap);

		for (int y = 0; y < MapGenerator.mapSize; y++) {
			for (int x = 0; x < MapGenerator.mapSize; x++) {
				float noisePoint = objectMap[x,y];
				Vector3 vertex = meshData.vertices [y * MapGenerator.mapSize + x];

				if (poissonDistribution[x,y]) {
					GameObject obj = (GameObject)Instantiate (spawnPrefab);
					obj.transform.position = new Vector3 (vertex.x + offset.x, vertex.y, vertex.z + offset.y);
					objList.Add (obj);
				}
			}
		}
			
		return objList;
	}

	private class Coord {
		public int x;
		public int y;

		public Coord(int x, int y) {
			this.x = x;
			this.y = y;
		}
		public float Distance(Coord other) {
			return Mathf.Sqrt(Mathf.Pow(this.x - other.x, 2) + Mathf.Pow(this.y - other.y, 2));
		}

	}
}
