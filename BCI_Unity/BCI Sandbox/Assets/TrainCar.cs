using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineFollower))]
public class TrainCar : MonoBehaviour {
	bool initialized = false;
	[SerializeField] [ReadOnly] SplineFollower mySplineFollower;
	[SerializeField] SplineFollower leader;
	[SerializeField] float horizOffset;

	private void Init() {
		if(initialized) return;
		mySplineFollower = this.GetComponent<SplineFollower>();
		initialized = true;
	}

	private void Start() {
		Init();
	}

	private void FixedUpdate() {
		this.mySplineFollower.distanceAlongSpline = Mathf.Clamp(leader.distanceAlongSpline-horizOffset, 0, float.MaxValue);
	}

}
