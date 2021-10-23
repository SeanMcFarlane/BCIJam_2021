using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Assets.LSL4Unity.Scripts.AbstractInlets;
using LSL;

/// <summary>
/// P300 Unity Inlet
/// Gathers information from Matlab and lights up the corresponding cube
/// </summary>
public class Inlet_P300_Event : AStringInlet {
	private string input = "";
	private double timestamp;
	private float freqHz;
	private List<GameObject> object_list;
	private Color onColour;
	private Color offColour;
	private double sim_start_time;
	private double sim_end_time;
	private int numRows;
	public int objectID;
	private int[] cubeIndices;

	bool initialized = false;
	P300_Controller p300Flashes;

	//Override the Process call from AInlet.CS
	protected override void Process(string[] newSample, double timeStamp) {
		//Avoid doing heavy processing here, use CoRoutines
		print("Input Received: " + input);
		input = newSample[0];
		timestamp = timeStamp;

		if(!initialized) {
			GameObject p300Controller = GameObject.FindGameObjectWithTag("BCI");
			p300Flashes = p300Controller.GetComponent<P300_Controller>();
			object_list = p300Flashes.object_list;
			freqHz = p300Flashes.freqHz;
			onColour = p300Flashes.onColour;
			offColour = p300Flashes.offColour;
			numRows = p300Flashes.numRows;
			cubeIndices = new int[numRows];
			initialized = true;
		}

		//Call CoRoutine to do further processing
		StartCoroutine("SelectedCube");
	}

	IEnumerator SelectedCube() {
		//print("Input Received: " + input + " at: " + timestamp);

		//Split the string into it's classifier and value
		string[] input_split = input.Split(',');
		string classifier = input_split[0];

		//What to do if the classifier is single target value.
		if(classifier == "s") {
			objectID = Int32.Parse(input_split[1]);
			print("\tSuccessful single target value?: " + objectID.ToString());

			//Using the P300 Event system!
			P300Events.current.TargetSelectionEvent(objectID);
		}
		else {
			print("Error: Classifier Value is: " + classifier);
			Debug.Log("Selected Object Id = " + objectID);
		}

		yield return new WaitForSecondsRealtime(2);
		TurnOff();


	}

	public void TurnOff() {
		for(int i = 0; i < object_list.Count; i++) {
			object_list[i].GetComponent<Renderer>().material.color = offColour;
		}
	}

	public void ResolveOnRequest() {
		liblsl.StreamInfo[] results;
		results = liblsl.resolve_streams();
		StartCoroutine("ResolveExpectedStream");
	}
}