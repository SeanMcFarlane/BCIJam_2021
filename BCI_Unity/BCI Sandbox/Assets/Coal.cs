using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coal : MonoBehaviour {
	public void GainCoal() {
		GameObject playerTrain = GameObject.Find("TrainEngine");
		TrainEngine trainEngine = playerTrain.GetComponent<TrainEngine>();
		trainEngine.coal++;
		Destroy(this.gameObject);
	}
}
