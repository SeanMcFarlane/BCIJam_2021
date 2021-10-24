using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameObjEvent : UnityEvent<GameObject> { }

public class Interactable : MonoBehaviour {

	/* 
	Place this component on any GameObject in the level that has a collider.
	Then, you can connect it to any other GameObject in the scene and execute public functions on them.
	This is useful for making any buttons, levers, doors, etc that need to interacted with.
	The trigger unityevents will only work if the collider is set to trigger mode.
	*/

	[SerializeField] [ReadOnlyAttribute] public bool initialized = false;

	[SerializeField] [ReadOnlyAttribute] public GameObject localPlayer;
	[SerializeField] private bool useMouseEvents = false;
	[SerializeField] private bool useTriggerEvents = false;
	[SerializeField] public GameObjEvent mouseEnterEvent;
	[SerializeField] public GameObjEvent mouseExitEvent;
	[SerializeField] public GameObjEvent interactEvent; //This is universal between mouse and BCI
	[SerializeField] public GameObjEvent enterTriggerEvent;
	[SerializeField] public GameObjEvent exitTriggerEvent;
	[SerializeField] [ReadOnlyAttribute] public SpriteRenderer mySprite;

	//Visual variables
	[SerializeField] [ReadOnlyAttribute] public Color myBaseColor = Color.white;
	[SerializeField] public Color myHighlightColor = Color.white;

	[SerializeField] public UnityEvent onStartEvent;
	[SerializeField] public GameObject interactor; //This is the entity that is highlighting this entity for an interaction. Null if no active interactor present.


	[Tooltip("Set to true if you want the sprite to display a highlighted outline when it is hovered over.")]
	[SerializeField] private bool useHighlightColor = false;
	[SerializeField] private bool isHighlighted = false;

	[SerializeField] [ReadOnly] private Sprite baseSprite;
	[SerializeField] private Sprite successSprite;

	[SerializeField] private float successSpriteDuration = 1f;
	[SerializeField] [ReadOnly] private float successSpriteTimer;


	#region BCI

	[SerializeField] [ReadOnlyAttribute] public int p300Id = -1;
	[SerializeField] [ReadOnlyAttribute] public P300_Controller controller;

	//A black then white flash to maximize contrast - just a random idea
	[SerializeField] private float blackFlashDuration = 0.01666f;
	[SerializeField] private float whiteFlashDuration = 0.03333f;

	[SerializeField] [ReadOnly] private float whiteFlashTimer;
	[SerializeField] [ReadOnly] private float blackFlashTimer;

	public void TriggerFlash() {
		whiteFlashTimer = whiteFlashDuration;
		blackFlashTimer = blackFlashDuration;
	}

	public void SuccessReactionVisual() {
		successSpriteTimer = successSpriteDuration;
	}

	#endregion

	void OnMouseEnter() {
		if(!useMouseEvents || !isActiveAndEnabled) return;
		FindPlayer();
		mouseEnterEvent.Invoke(localPlayer);
		Debug.Log("MOUSE ENTERED!");
		if(useHighlightColor && mySprite != null) {
			mySprite.color = myHighlightColor;
		}
	}

	void OnMouseExit() {
		if(!useMouseEvents || !isActiveAndEnabled) return;
		FindPlayer();
		mouseExitEvent.Invoke(localPlayer);
		Debug.Log("MOUSE EXIT!");
		if(useHighlightColor && mySprite != null) {
			mySprite.color = myBaseColor;
		}
	}

	void OnMouseDown() {
		if(!useMouseEvents || !isActiveAndEnabled) return;
		FindPlayer();
		interactEvent.Invoke(localPlayer);
		Debug.Log("MOUSE DOWN!");
	}

	void OnTriggerEnter2D(Collider2D col) {
		if(!useTriggerEvents || !isActiveAndEnabled) return;
		enterTriggerEvent.Invoke(col.gameObject);
	}

	void OnTriggerExit2D(Collider2D col) {
		if(!useTriggerEvents || !isActiveAndEnabled) return;
		exitTriggerEvent.Invoke(col.gameObject);
	}

	void OnP300Targeted(int id) {
		interactEvent.Invoke(localPlayer);
		Debug.Log("Brain click detected.");
	}

	private void Start() {
		Init();
	}

	public void Init() {
		if(initialized) return;
		initialized = true;

		mySprite = this.GetComponent<SpriteRenderer>();
		if(mySprite != null) {
			myBaseColor = mySprite.color;
			baseSprite = mySprite.sprite;
		}

		onStartEvent.Invoke();
		P300Events.current.OnTargetSelection += OnP300Targeted;
		P300_Controller controller = GameObject.Find("GameManager").GetComponent<P300_Controller>();
		controller.AddExistingBCIButton(this);
	}

	public void FindPlayer() {
		//Setting localPlayer here will allow interaction events to send the interacting player GameObject as an argument.
	}

	// Update is called once per frame
	void Update() {
		if(blackFlashTimer > 0) {
			mySprite.color = Color.black;
			blackFlashTimer -= Time.deltaTime;
		}
		else if(whiteFlashTimer > 0) {
			mySprite.color = Color.white;
			whiteFlashTimer -= Time.deltaTime;
		}
		else { //TODO: stop this from overriding hover highlight for mouse users
			mySprite.color = myBaseColor;
		}

		if(mySprite != null) {
			if(successSpriteTimer > 0) {
				mySprite.sprite = successSprite;
				successSpriteTimer -= Time.deltaTime;
			}
			else {
				mySprite.sprite = baseSprite;
			}
		}

	}
}


