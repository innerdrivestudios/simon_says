//Send by the trigger manager to IInteractables
public enum InteractableTrigger { 
	Enter,	//basically arming the trigger
	Exit,	//disarming the trigger
	Down,	//activating the trigger
	Up		//releasing the trigger (or triggering it based on how you handle the Down event)
};