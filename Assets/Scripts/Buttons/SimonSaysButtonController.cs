using System;
using UnityEngine;

[Tooltip(@"
/**
 * The SimonSaysButtonController is a mediator between the GameManager and different button view prefabs
 * instantiated at runtime. An instantiated button view prefab contains a collection of components that
 * extend ASimonSaysButtonView, for example a 'view' that plays audio, particle effects or animation effects.
 * In other words the controller has to gather and delegate trigger messages to MULTIPLE view elements.
 * 
 * The controller has a single kinematic rigidbody to make sure any child colliders are treated as a composite collider.
 */
")]
[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class SimonSaysButtonController : MonoBehaviour, IInteractable
{
	//Triggered if the button is triggered, static event so we don't have to know the source of the event, 
	//just register for this class and the actual controller instance will be passed in
	public static event Action<SimonSaysButtonController> OnTriggered = delegate { };

	//The game object that contains all of the view elements for a single button
	private GameObject buttonGO;
	//All of the view element components that are part of the button view gameobject
	private ASimonSaysButtonView[] viewElements;

	//Button controllers might be disabled/enabled based on game state and then still have to represent the correct state
	//(e.g. disable, mouseover, enable). In other words, we cannot simply ignore all interactions if we are not interactable,
	//we have to keep track of what our state would be as if we were interactable and then update as required based on the actual interactable state
	private bool over = false;
	private bool down = false;
	private bool interactable = true;

	private SimonSaysButtonDisplayState lastState = SimonSaysButtonDisplayState.Default;

	/**
	 * Instantiates the given prefab and then gathers references to all the ASimonSaysButtonView components
	 * on the newly created instance, passing on the controller's button index to each view component.
	 * ASimonSaysButtonView components can use this info to retrieve additional object information
	 * (e.g. colors, prefabs) from scriptable objects or whatever.
	 */
	public void InitializeNewView(GameObject pNewViewPrefab, int pButtonIndex)
	{
		if (buttonGO != null) Destroy(buttonGO);
		Debug.Log("Instantiating " + pNewViewPrefab);
		buttonGO = Instantiate(pNewViewPrefab, transform);

		viewElements = buttonGO.GetComponentsInChildren<ASimonSaysButtonView>();

		if (viewElements == null || viewElements.Length == 0)
		{
			Debug.LogWarning("There are 0 SimonSaysButtonView elements on the view prefab provided.");
			Debug.LogWarning("View will not respond to any state changes.");
		}
		else
		{
			//Since button prefabs might carry additional data with them, we'll tell them their index so they can use that
			//to retrieve the correct bit of data from any provided datasets
			foreach (ASimonSaysButtonView viewElement in viewElements) viewElement.SetButtonIndex(pButtonIndex); 
		}
	}

	public void ProcessTrigger(InteractableTrigger pTrigger)
	{
		switch (pTrigger)
		{
			case InteractableTrigger.Enter: 
				over = true; 
				break;
			case InteractableTrigger.Exit:	
				over = false; 
				break;
			case InteractableTrigger.Down:  
				down = true;
				break;
			case InteractableTrigger.Up:
				if (down && interactable)
				{
					OnTriggered.Invoke(this);
				}
				down = false; 
				break;
		}

		UpdateViewState();
	}

	/**
	 * Disables the interactable state and forces a display state on to the button.
	 */
	public void ForceState(SimonSaysButtonDisplayState pDisplayState)
	{
		UpdateViewState(pDisplayState);
	}

	/**
	 * Sets the interactable state of the button and makes sure the current viewstate matches the internal state of the controller.
	 */
	public void SetInteractable(bool pInteractable)
	{
		interactable = pInteractable;
		UpdateViewState();
	}

	/**
	 * Only updates the button if we are interactable. 
	 * Note that even though the controller/button might be in a not-interactable state,
	 * there is actually no DISABLED view state.
	 */
	private void UpdateViewState()
	{
		if (interactable)
		{
			if (down) UpdateViewState(SimonSaysButtonDisplayState.Activated);
			else if (over) UpdateViewState(SimonSaysButtonDisplayState.Hover);
			else UpdateViewState(SimonSaysButtonDisplayState.Default);
		}
	}

	/**
	 * Updates each of the ButtonView components on the view gameobject
	 */
	private void UpdateViewState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		if (viewElements == null || !gameObject.activeSelf || pNewDisplayState == lastState) return;
		lastState = pNewDisplayState;
		foreach (ASimonSaysButtonView viewElement in viewElements) viewElement.ChangeDisplayState(pNewDisplayState);
	}

}
