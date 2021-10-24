using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SplineFollower : MonoBehaviour {

	static int ARC_LENGTH_RESOLUTION = 1000;
	static float TANGENT_TEST_DELTA_POS = 0.1f;

	[SerializeField] [ReadOnly] private float angle;

	[SerializeField] private GameObject target;
	[SerializeField] [ReadOnly] private SpriteShapeController ssc;
	[SerializeField] [ReadOnly] private Spline spl;

	[SerializeField] private float vertOffset;

	[SerializeField] [ReadOnly] public float distanceAlongSpline;
	[SerializeField] [ReadOnly] private float localDistanceAlongSpline;

	[SerializeField] [ReadOnly] private List<float> segmentLengths;
	[SerializeField] [ReadOnly] private List<SplineArcLengthTable> splineArcLengthTables;


	// Start is called before the first frame update
	void Start() {
		ssc = target.GetComponent<SpriteShapeController>();
		spl = ssc.spline;
		Init();
	}

	void Init() {
		int pointCount = spl.GetPointCount();
		segmentLengths = new List<float>();
		splineArcLengthTables = new List<SplineArcLengthTable>();
		for(int i = 0; i<pointCount; ++i) {
			SplineArcLengthTable arcTable = SplineArcLengthTable.Generate(ARC_LENGTH_RESOLUTION, spl, i);
			float fullArcLength = arcTable.scalarToDistance(1.0f);
			segmentLengths.Add(fullArcLength);
			splineArcLengthTables.Add(arcTable);
		}
	}

	void FixedUpdate() {
		SetPositionOnSpline();
	}

	void SetPositionOnSpline() {
		float verticalOffset = ssc.colliderOffset + vertOffset;
		int pointCount = spl.GetPointCount();

		int currentSplineSection = -1;
		localDistanceAlongSpline = distanceAlongSpline;
		//Check each line segment length, remove it from total distance and go to next line segment until you find the segment you are on.
		for(int n = 0; n<segmentLengths.Count; ++n) {
			if(localDistanceAlongSpline < segmentLengths[n]) {//Found the section the player is currently on.
				currentSplineSection = n;
				break;
			}
			else localDistanceAlongSpline-=segmentLengths[n];//Move to next segment, remove current segment length from total distance
		}

		if(currentSplineSection == -1) {
			Debug.LogError("Could not locate position on spline. Probably exceeded total length.");
			return;
		}

		//Debug.Log("Located spline pos: Position ["+localDistanceAlongSpline.ToString("F2")+"] on spline index #"+currentSplineSection);

		int i = currentSplineSection;
		int spriteID = spl.GetSpriteIndex(i);
		if(spriteID == 2) {
			//Do something here to define when the next surface is not
			//return;
		}
		int prevIdx = i - 1;
		int curIdx = i;
		int nextIdx = i + 1;
		int nextNextIdx = i + 2;

		//This block handles looping past point 0 from both sides.(for closed splines)
		if(prevIdx< 0) { prevIdx = pointCount - 1; }
		if(prevIdx< 0 && ssc.spline.isOpenEnded) { Debug.LogError("Can't loop around on open ended splines."); }
		if(nextIdx == pointCount) {
			if(ssc.spline.isOpenEnded) return;
			else nextIdx = 0; // looping functionality
		}
		if(nextNextIdx >= pointCount) { nextNextIdx -= pointCount; }

		Vector3 prevPos = spl.GetPosition(prevIdx);
		Vector3 startPos = spl.GetPosition(curIdx);
		Vector3 endPos = spl.GetPosition(nextIdx);
		Vector3 nextPos = spl.GetPosition(nextNextIdx);

		Vector3 curLine = endPos - startPos;
		Vector3 prevLine = spl.GetLeftTangent(curIdx);
		Vector3 curLineLeft = spl.GetRightTangent(curIdx);
		Vector3 curLineRight = spl.GetLeftTangent(curIdx);
		Vector3 nextLine = spl.GetRightTangent(nextIdx);
		//If points are fixed, manually calculate their tangents
		if(prevLine == Vector3.zero) prevLine = prevPos - startPos;
		if(curLineLeft == Vector3.zero) curLineLeft = curLine;
		if(curLineRight == Vector3.zero) curLineRight = -curLine;
		if(nextLine == Vector3.zero) nextLine = nextPos - endPos;

		Vector3 startTangent = spl.GetRightTangent(curIdx) + startPos;
		Vector3 endTangent = spl.GetLeftTangent(nextIdx) + endPos;

		if(localDistanceAlongSpline <0 || localDistanceAlongSpline  > segmentLengths[i]) {
			Debug.LogError("Not within bounds of predicted spline segment.");
		}
		Vector3 curPoint = BezierUtility.BezierPoint(startPos, startTangent, endTangent, endPos, splineArcLengthTables[i].distanceToScalar(localDistanceAlongSpline));
		this.transform.position = curPoint;

		Vector3 prevPoint;
		prevPoint = BezierUtility.BezierPoint(startPos, startTangent, endTangent, endPos, splineArcLengthTables[i].distanceToScalar(localDistanceAlongSpline-TANGENT_TEST_DELTA_POS));

		Vector3 tangent = curPoint - prevPoint;
		if(tangent.magnitude > 0.001f) {
			angle = SplineUtils.Get2DAngle(tangent);
		}

		transform.rotation = Quaternion.Euler(0, 0, angle);
		transform.position += transform.up*verticalOffset;
	}
}


