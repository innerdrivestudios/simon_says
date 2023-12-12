using UnityEngine;
using UnityEngine.EventSystems;

/**
 * The TriggerManager has a single method: UpdateRay.
 * Based on your control mechanism (VR, Mouse, Touch) you should call UpdateRay with the correct input.
 * See MouseRay for an example.
 * 
 * As the ray is updated, the trigger manager keeps track of the last interactable it was targeting
 * and sends trigger messages to the IInteractable as appropriate.
 */
[DisallowMultipleComponent]
public class TriggerManager : MonoBehaviour
{
	[SerializeField] private LayerMask layerMask;
	[HideInInspector] public bool supportHover = false;

	private IInteractable lastInteractable = null;
	private bool lastRayActive = false;

	public void UpdateRay(Ray pRay, bool pRayActive)
	{
		//check what we are interacting with, ignoring rays that are aimed at any piece of UI
		IInteractable newInteractable = null;
		if (!EventSystem.current.IsPointerOverGameObject(Input.touchSupported?Input.GetTouch(0).fingerId:-1) && 
			Physics.Raycast(pRay, out RaycastHit hitInfo, float.PositiveInfinity, layerMask)
			) {
			newInteractable = hitInfo.rigidbody.GetComponent<IInteractable>();
		}

		//If nothing changed in our status, don't do anything, just exit
		if (newInteractable == lastInteractable && pRayActive == lastRayActive) return;

		//If I am interacting with something new, and haven't activated anything yet.... do the hover thingy and change the current interactable
		if (!lastRayActive && newInteractable != lastInteractable)
		{
			if (lastInteractable != null && supportHover) lastInteractable.ProcessTrigger(InteractableTrigger.Exit);
			if (newInteractable != null && supportHover) newInteractable.ProcessTrigger(InteractableTrigger.Enter);
			lastInteractable = newInteractable;
		}

		//if we were not interacting with something, but now we are, mark ourselves as active
		//of if we were interacting with something, but no longer, unmark ourselves as active
		if (lastRayActive != pRayActive)
		{
			lastRayActive = pRayActive;
			//make sure to inform the last set interactable of this new status
			lastInteractable?.ProcessTrigger(lastRayActive?InteractableTrigger.Down:InteractableTrigger.Up);
		}
	}
}
