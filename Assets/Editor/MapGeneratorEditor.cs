using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor {
	
	public override void OnInspectorGUI () {
		MapGenerator mapGen = (MapGenerator)target;

		if (DrawDefaultInspector () && mapGen.autoGen) {
			mapGen.ConstructMap (Vector2.zero);
		}

		if (GUILayout.Button ("Generate")) {
			mapGen.ConstructMap (Vector2.zero);
		}
	}
}
