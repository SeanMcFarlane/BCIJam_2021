using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryConditionTracker : MonoBehaviour {
	bool wonTheGame = false;

	[SerializeField] [ReadOnly] float totalTimePassed = 0;
	[SerializeField] float distanceToWin = 5175;

	[SerializeField] GameObject victoryScreen;
	[SerializeField] TextMeshProUGUI victoryScreenText;
	[SerializeField] SplineFollower trainSplineFollower;
	[SerializeField] TrainEngine train;

	[SerializeField] GameObject defeatScreen;
	[SerializeField] TextMeshProUGUI defeatScreenText;


	//Most of this is very lazy due to time constraints. TODO clean this up.
	public void FixedUpdate() {
		if(!train) train = FindObjectOfType<TrainEngine>();

		if(train.health <= 0) {
			defeatScreen.SetActive(true);
			defeatScreenText.text = "You may not have made it to gravy town, but you made it a whole "+(trainSplineFollower.distanceAlongSpline/1000f).ToString("0.0")+" kilometers!";
			trainSplineFollower.gameObject.GetComponent<TrainEngine>().movementSpeed = 0;
			trainSplineFollower.gameObject.GetComponent<TrainEngine>().currentThrottleLevel = 2;
			return;
		}

		totalTimePassed += Time.fixedDeltaTime;
		if(trainSplineFollower.distanceAlongSpline >= distanceToWin) {
			TrainEngine engine = trainSplineFollower.gameObject.GetComponent<TrainEngine>();
			if(!wonTheGame) {
				victoryScreen.SetActive(true);
				victoryScreenText.text = "You beat the game in "+totalTimePassed.ToString("0")+" seconds! By selling "+engine.gravy+" tons of gravy, you made "+50000*engine.gravy+" dollars!!";
				wonTheGame = true;
			}
			trainSplineFollower.distanceAlongSpline = distanceToWin;
			trainSplineFollower.gameObject.GetComponent<TrainEngine>().movementSpeed = 0;
			trainSplineFollower.gameObject.GetComponent<TrainEngine>().currentThrottleLevel = 2;
		}
	}

}
