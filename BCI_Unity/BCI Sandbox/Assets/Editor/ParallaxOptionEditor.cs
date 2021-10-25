/*
 THANKS TO DAVID DION-PAQUET AND HIS BLOG POST!
 http://www.gamasutra.com/blogs/DavidDionPaquet/20140601/218766/Creating_a_parallax_system_in_Unity3D_is_harder_than_it_seems.php
*/


using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParallaxOption))]
class ParallaxOptionEditor : Editor {

	private ParallaxOption options;

	void Awake() {
		options = (ParallaxOption)target;
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		if(GUILayout.Button("Save Position")) {
			options.SavePosition();
			EditorUtility.SetDirty(options);
		}

		if(GUILayout.Button("Restore Position"))
			options.RestorePosition();
	}
}