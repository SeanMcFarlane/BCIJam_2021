using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SplineFollower : MonoBehaviour {

	[SerializeField] private Spline targetSpline;

	[SerializeField] private float distanceAlongSpline;

	// Start is called before the first frame update
	void Start() {

	}

	void FixedUpdate() {
		//this.transform.position = targetSpline
	}
}
