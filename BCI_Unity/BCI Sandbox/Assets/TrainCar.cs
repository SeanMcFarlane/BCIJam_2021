using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineFollower))]
public class TrainCar : MonoBehaviour {
	bool initialized = false;
	[SerializeField] [ReadOnly] SplineFollower mySplineFollower;
	[SerializeField] SplineFollower leader;
	[SerializeField] float horizOffset;
	[SerializeField] float maximumDistanceToAttach = 6f; //If the car is further behind than this, it can't be grabbed anymore

	[SerializeField] private float jumpDelay;
	[SerializeField] private float jumpPower = 6;
	[SerializeField] [ReadOnly] private float jumpDelayTimer;

	[SerializeField] private bool attachedAtStart = false;

	[SerializeField] [ReadOnly] public TrainCar attachedCarBehind;



	private void Init() {
		if(initialized) return;
		mySplineFollower = this.GetComponent<SplineFollower>();
		initialized = true;
	}

	private void Start() {
		Init();
		if(attachedAtStart) {
			AttachToPlayerTrain();
		}
	}

	public void AttachToPlayerTrain() {
		GameObject playerTrain = GameObject.Find("TrainEngine");

		TrainEngine engine = playerTrain.GetComponent<TrainEngine>();

		if(engine.attachedCarBehind) {
			AttachToCargoCar(engine, engine.attachedCarBehind);
		}
		else {
			SplineFollower engineSplineFollower = playerTrain.GetComponent<SplineFollower>();
			if(engineSplineFollower.distanceAlongSpline-mySplineFollower.distanceAlongSpline > maximumDistanceToAttach) return;
			engine.attachedCarBehind = this;
			leader = playerTrain.GetComponent<SplineFollower>();
			engine.gravy++;
		}
	}

	private void AttachToCargoCar(TrainEngine playerTrain, TrainCar trainCar) {
		if(trainCar.attachedCarBehind) {
			AttachToCargoCar(playerTrain, trainCar.attachedCarBehind);
		}
		else {
			SplineFollower carSplineFollower = trainCar.GetComponent<SplineFollower>();
			if(carSplineFollower.distanceAlongSpline-mySplineFollower.distanceAlongSpline > maximumDistanceToAttach) return;
			trainCar.attachedCarBehind = this;
			leader = trainCar.GetComponent<SplineFollower>();
			playerTrain.gravy++;
		}
	}

	private void FixedUpdate() {
		if(!leader) return;

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

		if(this.mySplineFollower.distanceAlongSpline < leader.distanceAlongSpline-horizOffset) {
			this.mySplineFollower.distanceAlongSpline = Mathf.Clamp(leader.distanceAlongSpline-horizOffset, 0, float.MaxValue);
		}
	}

}
