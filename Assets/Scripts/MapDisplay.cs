using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

	public Renderer textureRenderer;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;

	public void DrawMesh (MeshData meshData, Texture2D texture) {
		meshFilter.gameObject.SetActive (true);
		meshFilter.sharedMesh = meshData.CreateMesh ();
		meshRenderer.sharedMaterial.mainTexture = texture;
		meshCollider.sharedMesh = meshFilter.sharedMesh;
	}

	public void Clear () {
		meshFilter.gameObject.SetActive (false);

	}
}
