using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : MonoBehaviour {
	bool initialized = false;
	[SerializeField] [ReadOnly] SplineFollower mySplineFollower;
	[SerializeField] [ReadOnly] Animator throttleAnimator;
	[SerializeField] [ReadOnly] Animator trainSpriteAnimator;

	//Visual tunings
	[SerializeField] private float fullWheelSpeedThreshold = 10f;

	//Jump parameters
	[SerializeField] private float jumpPower = 6;

	//Movespeed and acceleration
	[SerializeField] [ReadOnly] public float movementSpeed = 0;
	[SerializeField] private float maxSpeed = 50;
	[SerializeField] private float[] throttleLevels = { -2f, -1f, 0f, 0.25f, 0.5f };
	[SerializeField] public int currentThrottleLevel = 2; // acceleration of 0 by default

	public void Jump() {
		if(mySplineFollower.extraVertOffset >0) { return; }
		mySplineFollower.jumpVelocity = jumpPower;
	}

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
		Transform throttleTransform = transform.Find("TrainSprite/ThrottleIndicator");
		throttleAnimator = throttleTransform.GetComponent<Animator>();

		Transform trainSpriteTransform = transform.Find("TrainSprite");
		trainSpriteAnimator = trainSpriteTransform.GetComponent<Animator>();

		mySplineFollower = this.GetComponent<SplineFollower>();
		initialized = true;
	}

	private void Start() {
		Init();
	}

	private void Update() {
		throttleAnimator.speed = 0f;
		//Lazy way to select a visual state - pause the animation and set the desired keyframe directly. TODO do a proper state machine in the future.
		float animationFrameTime = 0.2f+ (Mathf.Round(((float)currentThrottleLevel/(float)throttleLevels.Length)*5f))/5f;
		throttleAnimator.Play("TrainThrottle", 0, animationFrameTime);

		trainSpriteAnimator.SetFloat("Speed", movementSpeed/fullWheelSpeedThreshold);
	}

	// Update is called once per frame
	void FixedUpdate() {
		movementSpeed += throttleLevels[currentThrottleLevel]*Time.fixedDeltaTime;
		mySplineFollower.distanceAlongSpline += movementSpeed*Time.fixedDeltaTime;
		movementSpeed = Mathf.Clamp(movementSpeed, 0, maxSpeed);
	}
}
