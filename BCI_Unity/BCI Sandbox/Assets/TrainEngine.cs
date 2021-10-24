using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : MonoBehaviour {
	bool initialized = false;
	[SerializeField] [ReadOnly] SplineFollower mySplineFollower;
	[SerializeField] private float movementSpeed = 6f;

	private void Init() {
		if(initialized) return;
		mySplineFollower = this.GetComponent<SplineFollower>();
		initialized = true;
	}

	private void Start() {
		Init();
	}

	// Update is called once per frame
	void FixedUpdate() {
		mySplineFollower.distanceAlongSpline += Time.fixedDeltaTime*movementSpeed;
	}
}
