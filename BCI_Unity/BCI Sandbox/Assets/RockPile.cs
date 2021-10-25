using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPile : MonoBehaviour {
	public void HitRock() {
		GameObject playerTrain = GameObject.Find("TrainEngine");
		TrainEngine trainEngine = playerTrain.GetComponent<TrainEngine>();
		trainEngine.TakeDamage(10);
		Destroy(this.gameObject);
	}
}
