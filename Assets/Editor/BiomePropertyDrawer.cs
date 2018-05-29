using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Biome))]
public class BiomePropertyDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty (position, label, property);
		Event currentEvent = Event.current;

		List<Biome> biomeList = (List<Biome>) fieldInfo.GetValue (property.serializedObject.targetObject);

		int index = Biome.index;
		Biome biome = biomeList [index];
		biome.Validate ();
		// calculate label width, allowing for a bit of a buffer on the right
		float labelWidth = GUI.skin.label.CalcSize (label).x + 5;

		Rect textureRect = new Rect (position.x + labelWidth, position.y, (position.width - labelWidth) / 2f, position.height);
		Rect threshRect = new Rect (textureRect.x + textureRect.width + 5, textureRect.y, textureRect.width / 2f, textureRect.height);

		if (currentEvent.type == EventType.Repaint) {
			// repaint
			// make a label (at "position", with string "label")
			GUI.Label (position, label);
			GUIStyle style = new GUIStyle ();
			Texture2D texture = biome.GetHorizTexture ((int)position.width);
			style.normal.background = texture;
			GUI.Label (textureRect, GUIContent.none, style);
			EditorGUI.PropertyField (threshRect, property.FindPropertyRelative ("threshold"), GUIContent.none);
		} else if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0) {
			// click
			if (textureRect.Contains (currentEvent.mousePosition)) {
				BiomeEditor window = EditorWindow.GetWindow<BiomeEditor> ();
				window.SetBiome (biome);
			}
		}
		biome.incrementIndex (biomeList.Count);
		EditorGUI.EndProperty();
	}
}
