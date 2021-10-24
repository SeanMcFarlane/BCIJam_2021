using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// This script connects a specific vertex of one spriteshape to a vertex on another spriteshape.
/// It forces those vertexes to line up exactly, and also informs spline followers to switch off to the new spriteshape at that point.
/// 
/// EDIT: This script now also tracks arc length data for interpolating along the spline, so the name is no longer accurate. 
/// It should be named "SplineDataCoordinator" or something like that. Will change later.
/// </summary>
[RequireComponent(typeof(SpriteShapeController))]
public class SplineConnector : MonoBehaviour {

	static int ARC_LENGTH_RESOLUTION = 1000;

	[SerializeField] [ReadOnly] private SpriteShapeController mySpline;
	[SerializeField] public SpriteShapeController targetNextSpline;
	[SerializeField] public int startVertex = 0;
	[SerializeField] public int sourceVertexIndex;
	[SerializeField] public int destinationVertexIndex;


	//Length tracking
	[SerializeField] [ReadOnly] public List<float> segmentLengths;
	[SerializeField] [ReadOnly] public List<SplineArcLengthTable> splineArcLengthTables;

	[SerializeField] [ReadOnly] public float lengthBeforeStart;
	[SerializeField] [ReadOnly] public float totalLength;
	[SerializeField] [ReadOnly] public float startDistance; //This is the distance this spline starts at, including distance from all previous tracks


	public void ConnectVertices(int sourceIndex, int destIndex) { //This must be done in the editor so that correct lengths are calculated at runtime
		Init();

		if(targetNextSpline.spline.GetTangentMode(destinationVertexIndex) != ShapeTangentMode.Broken || mySpline.spline.GetTangentMode(sourceVertexIndex) != ShapeTangentMode.Broken) {
			Debug.LogError("Must set the tangent mode of both points to 'broken' so that their tangents can match up.");
		}

		//Set destination vertex to have exact same position and tangent as source
		targetNextSpline.spline.SetPosition(destinationVertexIndex, mySpline.spline.GetPosition(sourceVertexIndex));
		targetNextSpline.spline.SetRightTangent(destinationVertexIndex, -mySpline.spline.GetLeftTangent(sourceVertexIndex));
		targetNextSpline.GetComponent<SplineConnector>().startVertex = destinationVertexIndex;
	}

	void Init() {
		mySpline = GetComponent<SpriteShapeController>();
		Spline spl = mySpline.spline;
		int pointCount = spl.GetPointCount();
		segmentLengths = new List<float>();
		splineArcLengthTables = new List<SplineArcLengthTable>();
		totalLength = 0;
		lengthBeforeStart = 0;
		for(int i = 0; i<pointCount; ++i) {
			if(i >= sourceVertexIndex) break;//Stop counting at the point where you are supposed to switch to the next SpriteShape.
			SplineArcLengthTable arcTable = SplineArcLengthTable.Generate(ARC_LENGTH_RESOLUTION, spl, i);
			float fullArcLength = arcTable.scalarToDistance(1.0f);
			segmentLengths.Add(fullArcLength);
			splineArcLengthTables.Add(arcTable);
			totalLength += fullArcLength;
			if(i < startVertex) {
				lengthBeforeStart+=fullArcLength;
			}
		}
		if(targetNextSpline != null) {
			SplineConnector targetSplineConnector = targetNextSpline.GetComponent<SplineConnector>();
			targetSplineConnector.SetStartDistance(totalLength+startDistance);
		}
	}

	public void SetStartDistance(float totalPrevLength) {
		startDistance = totalPrevLength;
		if(targetNextSpline != null) {

			if(targetNextSpline.GetComponent<SplineConnector>() == null) {
				Debug.LogError("Spline is missing a SplineConnector.", targetNextSpline.gameObject);
			}

			targetNextSpline.GetComponent<SplineConnector>().SetStartDistance(totalLength+startDistance);
		}
	}

	// Start is called before the first frame update
	void Start() {
		Init();
	}

	// Update is called once per frame
	void Update() {

	}
}
