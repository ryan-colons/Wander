using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandEnd : MonoBehaviour {

	private Coord prevCoord;
	private Camera camera;
	private SpellCoder spellCoder;

	public GameObject projectileSpawnPoint;

	private void Start () {
		Spell.projectileSpawn = projectileSpawnPoint;
	}

	private void Awake () {
		prevCoord = new Coord (-1, -1);
		camera = FindObjectOfType<Camera> ();
		spellCoder = FindObjectOfType<SpellCoder> ();
	}

	private void Update () {
		Vector3 screenPos = camera.WorldToViewportPoint (transform.position);
		Coord coord = ScreenPosToCoord (screenPos);
		if (!coord.Equals (prevCoord)) {
			prevCoord = coord;
			spellCoder.ReceiveCode (CoordToNum (coord));
		}
	}

	private Coord ScreenPosToCoord (Vector3 screenPos) {
		float chunkPercent = 1f / SpellCoder.codeAlphabetLength;
		float x = Mathf.Clamp (screenPos.x, 0f, 1f);
		float y = Mathf.Clamp (screenPos.y, 0f, 1f);
		int xPos = (int)(x / chunkPercent);
		int yPos = (int)(y / chunkPercent);

		return new Coord (xPos, yPos);;
	}

	private int CoordToNum (Coord coord) {
		return SpellCoder.codeAlphabetLength * coord.x + coord.y;
	}

	private struct Coord {
		public readonly int x;
		public readonly int y;

		public Coord (int x, int y) {
			this.x = x;
			this.y = y;
		}

		public void Print () {
			Debug.Log (x + ", " + y);
		}

		public bool Equals (Coord other) {
			return (other.x == this.x && other.y == this.y) ? true : false;
		}
	}

}
