using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineFollower))]
public class TrainCar : MonoBehaviour {
	bool initialized = false;
	[SerializeField] [ReadOnly] SplineFollower mySplineFollower;
	[SerializeField] SplineFollower leader;
	[SerializeField] float horizOffset;

	[SerializeField] private float jumpDelay;
	[SerializeField] private float jumpPower = 6;
	[SerializeField] [ReadOnly] private float jumpDelayTimer;


	private void Init() {
		if(initialized) return;
		mySplineFollower = this.GetComponent<SplineFollower>();
		initialized = true;
	}

	private void Start() {
		Init();
	}

	private void FixedUpdate() {
		if(jumpDelayTimer == 0 && mySplineFollower.extraVertOffset <= 0 && leader.jumpVelocity > 0) {
			jumpDelayTimer = jumpDelay;
		}

		if(jumpDelayTimer > 0) {
			jumpDelayTimer-=Time.fixedDeltaTime;
			if(jumpDelayTimer <=0) {
				mySplineFollower.jumpVelocity = jumpPower;
				jumpDelayTimer = 0;
			}
		}

		this.mySplineFollower.distanceAlongSpline = Mathf.Clamp(leader.distanceAlongSpline-horizOffset, 0, float.MaxValue);
	}

}
