using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class InfiniteTerrain : MonoBehaviour {

	public const float maxViewDst = 200;
	public Transform viewer;
	public static Vector2 viewerPosition;
	public Material terrainMaterial;

	private static ObjectGenerator objGenerator;
	private static MapGenerator mapGenerator;
	public int chunkSize;
	private int chunksVisible;

	private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	private List<TerrainChunk> chunksVisibleLastUpdate = new List<TerrainChunk>();

	private void Start () {
		objGenerator = FindObjectOfType<ObjectGenerator> ();
		mapGenerator = FindObjectOfType<MapGenerator> ();
		chunkSize = MapGenerator.mapSize - 1;
		chunksVisible = Mathf.RoundToInt (maxViewDst / chunkSize);
	}

	private void Update () {
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);
		UpdateVisibleChunks ();

		if (chunkRedrawQueue.Count > 0) {
			ChunkRedrawInfo redrawInfo = chunkRedrawQueue [0];
			Texture2D texture = TextureGenerator.GenerateTextureFromColorMap(redrawInfo.colours, chunkSize + 1, chunkSize + 1);
			redrawInfo.chunk.RedrawTexture (texture);
			chunkRedrawQueue.Remove (redrawInfo);
		}
	}

	private void UpdateVisibleChunks () {
		foreach (TerrainChunk chunk in chunksVisibleLastUpdate) {
			chunk.SetVisible (false);
		}
		chunksVisibleLastUpdate.Clear ();

		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);

		for (int yOffset = -chunksVisible; yOffset <= chunksVisible; yOffset++) {
			for (int xOffset = -chunksVisible; xOffset <= chunksVisible; xOffset++) {
				Vector2 visibleChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (terrainChunkDictionary.ContainsKey (visibleChunkCoord)) {
					terrainChunkDictionary [visibleChunkCoord].UpdateTerrainChunk ();
					if (terrainChunkDictionary [visibleChunkCoord].IsVisible ()) {
						chunksVisibleLastUpdate.Add (terrainChunkDictionary [visibleChunkCoord]);
					}
				} else {
					terrainChunkDictionary.Add(visibleChunkCoord, new TerrainChunk(visibleChunkCoord, chunkSize, this.transform, terrainMaterial));
				}
			}
		}
	}

	// only here for the BiomeShift spell, which is slightly gross
	private List<ChunkRedrawInfo> chunkRedrawQueue = new List<ChunkRedrawInfo>();

	public void RedrawAllTextures () {
		foreach (KeyValuePair<Vector2, TerrainChunk> kvp in terrainChunkDictionary) {
			TerrainChunk chunk = kvp.Value;
			ThreadStart threadStart = delegate {
				MapData mapData = chunk.GenerateMapData();
				ChunkRedrawInfo redrawInfo = new ChunkRedrawInfo(chunk, mapData.colorMap);
				lock(chunkRedrawQueue) {
					chunkRedrawQueue.Add(redrawInfo);
				}
			};
			new Thread (threadStart).Start ();
		}
		for (int i = 1; i < chunkRedrawQueue.Count; i++) {
			ChunkRedrawInfo info = chunkRedrawQueue [i];
			if (info.chunk != null && info.chunk.IsVisible ()) {
				chunkRedrawQueue.Remove (info);
				chunkRedrawQueue.Insert (0, info);
			}
		}
	}

	private struct ChunkRedrawInfo {
		public TerrainChunk chunk;
		public Color[] colours;
		public ChunkRedrawInfo (TerrainChunk chunk, Color[] colours) {
			this.chunk = chunk;
			this.colours = colours;
		}
	}

	public class TerrainChunk {
		GameObject meshObject;
		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		MeshCollider meshCollider;
		Vector2 position;
		Bounds bounds;
		
		public TerrainChunk(Vector2 coord, int size, Transform parent, Material material) {
			position = coord * size;
			bounds = new Bounds (position, Vector2.one * size);
			Vector3 positionInWorld = new Vector3 (position.x, 0, position.y);

			meshObject = new GameObject ("Terrain Chunk");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshCollider = meshObject.AddComponent<MeshCollider>();
			meshRenderer.material = material;

			meshObject.transform.position = positionInWorld;
			meshObject.transform.parent = parent;
			SetVisible(false);

			MapData mapData = GenerateMapData();
			MeshData meshData = MeshGenerator.GenerateMesh(mapData.heightMap, mapGenerator.terrain.meshHeightMultiplier, mapGenerator.terrain.meshHeightCurve);

			meshFilter.mesh = meshData.CreateMesh ();
			meshRenderer.material.mainTexture = TextureGenerator.GenerateTextureFromColorMap(mapData.colorMap, size + 1, size + 1);
			meshCollider.sharedMesh = meshFilter.mesh;

			List<GameObject> objList = objGenerator.GenerateObjects(meshData, mapData.objectMap, position);
			for (int i = 0; i < objList.Count; i++) {
				objList[i].transform.parent = meshObject.transform;
			}
		}

		public MapData GenerateMapData () {
			return mapGenerator.GenerateMapData(position);
		}

		public void RedrawTexture (Texture2D texture) {
			meshRenderer.material.mainTexture = texture;
		}

		public void UpdateTerrainChunk () {
			float dstFromViewer = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));
			bool visible = dstFromViewer <= maxViewDst;
			this.SetVisible (visible);
		}

		public void SetVisible (bool visible) {
			meshObject.SetActive (visible);
		}

		public bool IsVisible () {
			return meshObject.activeSelf;
		}
	}
}
