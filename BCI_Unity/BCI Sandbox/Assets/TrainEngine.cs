using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : MonoBehaviour {
	bool initialized = false;
	[SerializeField] [ReadOnly] SplineFollower mySplineFollower;
	[SerializeField] private float movementSpeed = 0;

	[SerializeField] private float[] throttleLevels = { -1, -0.5f, 0, 0.5f, 1 };
	[SerializeField] private int currentThrottleLevel = 2; // acceleration of 0 by default

	public void ThrottleUp() {
		if(currentThrottleLevel < throttleLevels.Length-1) {
			currentThrottleLevel++;
		}
	}

	public void ThrottleDown() {
		if(currentThrottleLevel >0) {
			currentThrottleLevel--;
		}
	}

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
		movementSpeed += throttleLevels[currentThrottleLevel]*Time.fixedDeltaTime;
		mySplineFollower.distanceAlongSpline += movementSpeed*Time.fixedDeltaTime;
		Mathf.Clamp(movementSpeed, 0, 100f);
	}
}
