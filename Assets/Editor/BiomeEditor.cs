using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BiomeEditor : EditorWindow {

	Biome biome;
	const int borderSize = 10;
	const float keyWidth = 20;
	const float keyHeight = 10;

	private Rect biomeRect;
	private Rect settingsRect;
	private Rect[] keyRects;
	private int selectedKeyIndex = 0;
	private bool mouseDownOverKey;

	private bool needsRepaint;

	private void OnGUI () {
		Draw ();
		HandleInput ();

		if (needsRepaint) {
			Repaint ();
			needsRepaint = false;
		}
			
		GUILayout.BeginArea (new Rect (settingsRect.x, settingsRect.yMin + 50, 300, 300));
		if (GUILayout.Button ("Move Up", GUILayout.MaxWidth(250))) {
			MapGenerator mapGen = FindObjectOfType<MapGenerator> ();
			int index = mapGen.biomes.IndexOf (biome);
			if (index > 0) {
				mapGen.biomes.Remove (biome);
				mapGen.biomes.Insert (index - 1, biome);
			}
		}
		if (GUILayout.Button ("Move Down", GUILayout.MaxWidth (250))) {
			MapGenerator mapGen = FindObjectOfType<MapGenerator> ();
			int index = mapGen.biomes.IndexOf (biome);
			if (index < mapGen.biomes.Count - 1) {
				mapGen.biomes.Remove (biome);
				mapGen.biomes.Insert (index + 1, biome);
			}
		}
		if (GUILayout.Button ("Duplicate", GUILayout.MaxWidth (250))) {
			MapGenerator mapGen = FindObjectOfType<MapGenerator> ();
			Biome newBiome = new Biome (biome.regions, biome.threshold);
			int index = mapGen.biomes.IndexOf (biome);
			mapGen.biomes.Insert (index, newBiome);
		}
		if (GUILayout.Button ("Delete!", GUILayout.MaxWidth (200))) {
			MapGenerator mapGen = FindObjectOfType<MapGenerator> ();
			int index = mapGen.biomes.IndexOf (biome);			
			if (index != -1) {
				mapGen.biomes.Remove (biome);
				Close ();
			}
		}
		GUILayout.EndArea ();
	}

	private void Draw () {
		biomeRect = new Rect (borderSize, borderSize, 50, position.height - (borderSize * 2));
		GUI.DrawTexture(biomeRect, biome.GetVertTexture((int)biomeRect.height));

		keyRects = new Rect[biome.regions.Count];
		for (int i = 0; i < keyRects.Length; i++) {
			TerrainType terrain = biome.regions [i];
			Rect keyRect = new Rect (biomeRect.xMax + borderSize, biomeRect.y + biomeRect.height * (1f - terrain.height) - (keyHeight / 2f), keyWidth, keyHeight);
			if (i == selectedKeyIndex) {
				EditorGUI.DrawRect (new Rect (keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
			}
			EditorGUI.DrawRect (keyRect, terrain.colour);
			keyRects [i] = keyRect;
		}

		settingsRect = new Rect (keyRects [0].xMax + borderSize, borderSize, position.width, position.height);
		GUILayout.BeginArea (settingsRect);
		EditorGUI.BeginChangeCheck ();
		Color newColour = EditorGUILayout.ColorField (biome.regions [selectedKeyIndex].colour, GUILayout.MaxWidth(250));
		if (EditorGUI.EndChangeCheck ()) {
			biome.SetRegionColour (selectedKeyIndex, newColour);
		}
		biome.threshold = EditorGUILayout.FloatField("Threshold", biome.threshold, GUILayout.MaxWidth(200));
		GUILayout.EndArea ();
	}

	private void HandleInput () {
		Event currentEvent = Event.current;
		if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0) {
			// check if we've clicked an existing key
			for (int i = 0; i < keyRects.Length; i++) {
				if (keyRects [i].Contains (currentEvent.mousePosition)) {
					selectedKeyIndex = i;
					mouseDownOverKey = true;
					break;
				}
			}
			// add a new key, if we're not clicking on an existing one
			if (!mouseDownOverKey && biomeRect.Contains(currentEvent.mousePosition)) {
				float newHeight = Mathf.InverseLerp (biomeRect.yMax, biomeRect.y, currentEvent.mousePosition.y);
				Color newColour = biome.EvaluateToColour (newHeight);
				selectedKeyIndex = biome.AddRegion (newHeight, newColour);
				mouseDownOverKey = true;
				needsRepaint = true;
			}
		}
		if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0) {
			mouseDownOverKey = false;
		}
		if (mouseDownOverKey && currentEvent.type == EventType.MouseDrag && currentEvent.button == 0) {
			float newHeight = Mathf.InverseLerp (biomeRect.yMax, biomeRect.y, currentEvent.mousePosition.y);
			selectedKeyIndex = biome.SetRegionHeight (selectedKeyIndex, newHeight);
			needsRepaint = true;
		}
		if (currentEvent.keyCode == KeyCode.Backspace && currentEvent.type == EventType.KeyDown) {
			biome.RemoveRegion (selectedKeyIndex);
			if (selectedKeyIndex >= biome.regions.Count) {
				selectedKeyIndex -= 1;
			}
			needsRepaint = true;
		}
	}

	public void SetBiome (Biome biome) {
		this.biome = biome;
	}

	private void OnDisable () {
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEngine.SceneManagement.SceneManager.GetActiveScene ());
	}
}
