using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class SplineUtils {
	public static float Get2DAngle(Vector3 vector3) {
		float angle = Mathf.Atan2(vector3.x, vector3.y) * Mathf.Rad2Deg;
		angle = 90 - angle;
		if(angle > 180)
			angle = -360 + angle;
		return angle;
	}
}


//This is a very lazy and unoptimized implementation; use as little as possible during runtime.
//Built on top of UnityEngine.U2D package for splines.
//Arc length parameterization to ensure smooth, linear interpolation over the length of a spline.
public struct SplineArcLengthTable {
	public float[] arc_lengths;
	private int ARC_LENGTH_ITERATIONS;

	public SplineArcLengthTable(float[] newTable, int arcLengthIterations) {
		arc_lengths = newTable;
		ARC_LENGTH_ITERATIONS = arcLengthIterations;
	}

	public static SplineArcLengthTable Generate(int arcLengthIterations, Spline spl, int i) {
		int pointCount = spl.GetPointCount();
		int curIdx = i;
		int nextIdx = i + 1;
		if(nextIdx == pointCount) {
			if(spl.isOpenEnded) {
				Debug.LogError("Index out of bounds for open ended splines.");
				return new SplineArcLengthTable();
			}
			nextIdx = 0; // looping functionality
		}

		Vector3 startPos = spl.GetPosition(curIdx);
		Vector3 endPos = spl.GetPosition(nextIdx);
		Vector3 startTangent = spl.GetRightTangent(curIdx) + startPos;
		Vector3 endTangent = spl.GetLeftTangent(nextIdx) + endPos;

		List<float> new_arc_lengths = new List<float>();
		new_arc_lengths.Add(0.0f);

		for(int j = 1; j <= arcLengthIterations; j++) {

			float lastVal = (float)(j - 1) / (float)arcLengthIterations;
			float curVal = (float)j / (float)arcLengthIterations;

			Vector3 delta = BezierUtility.BezierPoint(startPos, startTangent, endTangent, endPos, curVal) - BezierUtility.BezierPoint(startPos, startTangent, endTangent, endPos, lastVal);
			float length = delta.magnitude;
			float cumulativeLength = new_arc_lengths.Last() + length;
			new_arc_lengths.Add(cumulativeLength);
		}
		if(new_arc_lengths.Count != arcLengthIterations + 1) { Debug.LogError("new_arc_lengths.Count == arcLengthIterations + 1"); }
		return new SplineArcLengthTable(new_arc_lengths.ToArray(), arcLengthIterations);
	}

	public float scalarToDistance(float val) {
		if(val < 0 || val > 1) {
			Debug.LogError("Invalid arc length specified:" + val);
		}

		int i = (int)(val * (float)ARC_LENGTH_ITERATIONS);
		float t = (val * (float)ARC_LENGTH_ITERATIONS) - (float)i;
		float prev_length = arc_lengths[i];
		float next_length_delta = 0;
		if(i < ARC_LENGTH_ITERATIONS) {
			next_length_delta = arc_lengths[i + 1] - arc_lengths[i];
		}
		return prev_length + (next_length_delta * t);
	}

	public float distanceToScalar(float distance) {
		float uVal = 0;
		int uIndex = -1;
		int i = 1;

		if(arc_lengths.Length <= 0) return 0;
		for(i = 1; i < arc_lengths.Length; i++) {
			if(distance < arc_lengths[i]) {
				uIndex = i - 1;
				break;
			}
		}

		if(uIndex == -1) { //Exceeded maximum range - clamping.
			uIndex = arc_lengths.Length - 1;
		}

		if(uIndex + 1 == arc_lengths.Length) {
			//Reached the end of the path.
			uVal = 1.0f;
		}
		else {
			float prevLength = arc_lengths[uIndex];
			float nextLength = arc_lengths[uIndex + 1];
			float localInterp = (distance - prevLength) / (nextLength - prevLength);
			uVal = ((float)uIndex + localInterp) / (float)(arc_lengths.Length);
		}

		if(uVal > 1.0f) uVal = 1.0f;
		return uVal;
	}

}

