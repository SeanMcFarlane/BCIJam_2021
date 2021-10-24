using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineConnector))]
public class SplineConnectorEditor : Editor {
	private SplineConnector myConnector;

	void Awake() {
		myConnector = (SplineConnector)target;
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		if(GUILayout.Button("Connect Splines at marked vertices")) {
			myConnector.ConnectVertices(myConnector.sourceVertexIndex, myConnector.destinationVertexIndex);
			EditorUtility.SetDirty(this);
		}
	}

}
