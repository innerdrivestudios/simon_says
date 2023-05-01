/**
 * Interface for objects detected by the trigger manager.
 * Interactables should implement this interface and have a collider with a layer that matches the TriggerManager's LayerMask setting,
 * in order to receive trigger signals.
 */
public interface IInteractable
{
	void ProcessTrigger(InteractableTrigger pTrigger);
}
