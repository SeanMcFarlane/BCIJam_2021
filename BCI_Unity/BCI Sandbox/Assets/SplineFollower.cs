using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SplineFollower : MonoBehaviour {
	bool initialized = false;

	static float TANGENT_TEST_DELTA_POS = 0.1f;

	[SerializeField] [ReadOnly] public float angle;

	[SerializeField] private GameObject target;
	[SerializeField] [ReadOnly] private SplineConnector targetSC;
	[SerializeField] [ReadOnly] private SpriteShapeController ssc;
	[SerializeField] [ReadOnly] private Spline spl;

	[SerializeField] private float vertOffset;

	//Jump parameters
	[SerializeField] private float jumpGravity = 9.86f;
	[SerializeField] [ReadOnly] public float jumpVelocity;
	[SerializeField] [ReadOnly] public float extraVertOffset;

	[SerializeField] public float distanceAlongSpline;
	[SerializeField] [ReadOnly] private float localDistanceAlongSpline;

	void SetTarget(GameObject newTarget) {

		Debug.Log("Set new spline target: "+newTarget.name, newTarget);
		target = newTarget;
		ssc = target.GetComponent<SpriteShapeController>();
		spl = ssc.spline;
		targetSC = target.GetComponent<SplineConnector>();
	}

	// Start is called before the first frame update
	void Start() {
		Init();
	}

	void Init() {
		if(initialized) return;
		initialized = true;
		SetTarget(target);
	}

	void FixedUpdate() {
		jumpVelocity -= jumpGravity*Time.fixedDeltaTime;
		extraVertOffset+=jumpVelocity*Time.fixedDeltaTime;
		extraVertOffset = Mathf.Clamp(extraVertOffset, 0, float.MaxValue);
		SetPositionOnSpline();
	}

	void SetPositionOnSpline() {
		if(distanceAlongSpline > targetSC.startDistance+targetSC.totalLength) {
			SetTarget(targetSC.targetNextSpline.gameObject);//Move to next Spline altogether.
		}

		float verticalOffset = ssc.colliderOffset + vertOffset + extraVertOffset;
		int pointCount = spl.GetPointCount();

		int currentSplineSection = -1;
		//Subtract all previous spriteshapes traversed, and add some length to skip to the right spot on the next spline
		localDistanceAlongSpline = distanceAlongSpline - targetSC.startDistance;

		//Check each line segment length, remove it from total distance and go to next line segment until you find the segment you are on.
		for(int n = targetSC.startVertex; n<targetSC.segmentLengths.Count; n++) {
			if(localDistanceAlongSpline < targetSC.segmentLengths[n]) {//Found the section the player is currently on.
				currentSplineSection = n;
				break;
			}
			else localDistanceAlongSpline-=targetSC.segmentLengths[n];//Move to next segment, remove current segment length from total distance
		}

		//Debug.Log("TotalDistance["+distanceAlongSpline+"]");
		//Debug.Log("Spline Position ["+localDistanceAlongSpline.ToString("F2")+"] on spline index #"+currentSplineSection);

		if(currentSplineSection == -1) {
			Debug.LogError("Could not locate position on spline. Probably exceeded total length.");
			return;
		}

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

		if(localDistanceAlongSpline <0 || localDistanceAlongSpline  > targetSC.segmentLengths[i]) {
			Debug.LogError("Not within bounds of predicted spline segment.");
		}
		Vector3 curPoint = BezierUtility.BezierPoint(startPos, startTangent, endTangent, endPos, targetSC.splineArcLengthTables[i].distanceToScalar(localDistanceAlongSpline));
		this.transform.position = curPoint;

		Vector3 prevPoint;
		prevPoint = BezierUtility.BezierPoint(startPos, startTangent, endTangent, endPos, targetSC.splineArcLengthTables[i].distanceToScalar(localDistanceAlongSpline-TANGENT_TEST_DELTA_POS));

		Vector3 tangent = curPoint - prevPoint;
		if(tangent.magnitude > 0.001f) {
			angle = SplineUtils.Get2DAngle(tangent);
		}

		transform.rotation = Quaternion.Euler(0, 0, angle);
		transform.position += transform.up*verticalOffset;
	}
}


