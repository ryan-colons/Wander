using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Spell_DrawLine : Spell {

	private LineRenderer lineRenderer;
	private List<GameObject> linePoints;
	public GameObject lineCornerMarker;
	public int maxPoints;
	public float spawnDistance;

	private void Start () {
		lineRenderer = this.GetComponent<LineRenderer> ();
		linePoints = new List<GameObject> ();
	}

	public override void Cast () {
		SpawnNewLinePoint ();
		Vector3[] positions = new Vector3[linePoints.Count];
		for (int i = 0; i < linePoints.Count; i++) {
			positions [i] = linePoints [i].transform.position;
		}
		lineRenderer.widthMultiplier = 1f / (linePoints.Count + 1);
		lineRenderer.positionCount = linePoints.Count;
		lineRenderer.SetPositions (positions);
	}

	private void SpawnNewLinePoint () {
		if (linePoints.Count > maxPoints) {
			ClearLinePoints ();
		}
		Vector3 spawnPos = projectileSpawn.transform.position + projectileSpawn.transform.forward * spawnDistance;
		GameObject marker = (GameObject)Instantiate (lineCornerMarker, this.transform);
		marker.transform.position = spawnPos;
		linePoints.Add (marker);
	}

	private void ClearLinePoints () {
		foreach (GameObject obj in linePoints) {
			Destroy (obj);
		}
		linePoints = new List<GameObject> ();
	}

}