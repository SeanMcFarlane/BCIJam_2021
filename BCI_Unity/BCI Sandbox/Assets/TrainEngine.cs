using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : MonoBehaviour {
	bool initialized = false;
	[SerializeField] [ReadOnly] SplineFollower mySplineFollower;
	[SerializeField] [ReadOnly] Animator throttleAnimator;
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
		//TrainSprite/ThrottleIndicator
		Transform throttleTransform = transform.Find("TrainSprite/ThrottleIndicator");
		throttleAnimator = throttleTransform.GetComponent<Animator>();
		mySplineFollower = this.GetComponent<SplineFollower>();
		initialized = true;
	}

	private void Start() {
		Init();
	}

	private void Update() {
		throttleAnimator.speed = 0f;
		//Lazy way to select a visual state - pause the animation and set the desired keyframe directly. TODO do a proper state machine in the future.
		float animationFrameTime = 0.0999f+(float)currentThrottleLevel/(float)throttleLevels.Length;
		throttleAnimator.Play("TrainThrottle", 0, animationFrameTime);
	}

	// Update is called once per frame
	void FixedUpdate() {
		movementSpeed += throttleLevels[currentThrottleLevel]*Time.fixedDeltaTime;
		mySplineFollower.distanceAlongSpline += movementSpeed*Time.fixedDeltaTime;
		movementSpeed = Mathf.Clamp(movementSpeed, 0, 100f);
	}
}
