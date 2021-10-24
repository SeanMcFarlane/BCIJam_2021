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

	public void FixedUpdate() {
		totalTimePassed += Time.fixedDeltaTime;
		if(trainSplineFollower.distanceAlongSpline >= distanceToWin) {
			if(!wonTheGame) {
				victoryScreen.SetActive(true);
				victoryScreenText.text = "You beat the game in "+totalTimePassed.ToString("0")+" seconds!";
				wonTheGame = true;
			}
			trainSplineFollower.distanceAlongSpline = distanceToWin;
			trainSplineFollower.gameObject.GetComponent<TrainEngine>().movementSpeed = 0;
			trainSplineFollower.gameObject.GetComponent<TrainEngine>().currentThrottleLevel = 2;
		}
	}

}
