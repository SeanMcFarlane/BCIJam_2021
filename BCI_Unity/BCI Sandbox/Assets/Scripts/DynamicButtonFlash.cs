using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DynamicButtonFlash : MonoBehaviour {
	//Main connection to the P300 controller.
	[SerializeField] [ReadOnly] P300_Controller p300_Controller;

	public bool active = false;
	[ReadOnly] public bool wasActive = false;

	[SerializeField] float flashInterval = 1f;
	[SerializeField] [ReadOnly] float flashTimer = 0f;


	//Variables associated with counting the flashes
	private int counter = 0;
	private List<Interactable> buttonsToFlash = new List<Interactable>();
	private int numTrials = 0;

	private void Awake() {
		p300_Controller = GetComponent<P300_Controller>();
	}

	public void RefillFlashQueue() {
		//Add interactables multiple times potentially, depending on max samples.
		//Flash a random one each time, and remove it from the list until the list is empty.
		//Then it will be repopulated from the master list.

		int numSamples = p300_Controller.numFlashes;

		buttonsToFlash.Clear();
		for(int i = 0; i < numSamples; i++) {
			//Add the entire list multiple times, so that each interactable gets flashed i times.
			buttonsToFlash.AddRange(p300_Controller.bciButtonList);
		}

		numTrials = numSamples * p300_Controller.bciButtonList.Count;

		print("---------- DYNAMIC FLASH DETAILS ----------");
		print("Number of Trials will be: " + numTrials);
		print("Number of flashes for each cell: " + numSamples);
		print("--------------------------------------");
		TurnOffAllDynamic();
	}

	public void Update() {
		if(active && p300_Controller.bciButtonList.Count > 0) {
			if(!wasActive) {
				wasActive = true;
				p300_Controller.WriteMarker("P300 Dynamic Flash Started");
			}

			flashTimer -= Time.deltaTime;
			if(flashTimer <= 0) {
				//Activation time for next button
				DoNextFlash();
				flashTimer = flashInterval; //reset timer
			}
		}
		else if(wasActive) {
			wasActive = false;
			p300_Controller.WriteMarker("P300 Dynamic Flash Ends");
		}
	}

	//Lazy and maybe unnecessary, but check for and remove destroyed interactables from the list every time before we do a flash.
	public void UpkeepButtonsToFlashList() {
		for(int i = buttonsToFlash.Count-1; i >=0; i--) {//Iterating downwards to ensure correct memory access when removing elements
			if(buttonsToFlash[i] == null) {
				buttonsToFlash.RemoveAt(i);
			}
		}
	}

	private void DoNextFlash() {
		UpkeepButtonsToFlashList();
		if(buttonsToFlash.Count <= 0) {
			RefillFlashQueue();
		}

		int randomIndex = Random.Range(0, buttonsToFlash.Count);
		//Write to the LSL Outlet stream
		p300_Controller.WriteMarker("s," + randomIndex.ToString());
		if(buttonsToFlash[randomIndex] == null) Debug.LogError("Button at index "+randomIndex+" is missing!");
		buttonsToFlash[randomIndex].TriggerFlash();
		buttonsToFlash.RemoveAt(randomIndex);
	}

	//Turn off all object values
	public void TurnOffAllDynamic() {
		for(int i = 0; i < p300_Controller.object_list.Count; i++) {
			p300_Controller.object_list[i].GetComponent<Renderer>().material.color = p300_Controller.offColour;
		}
	}
}
