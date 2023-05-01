/**
 * The SimonSaysButtonDisplayState is similar/related to the InteractableTrigger, 
 * IInteractables like the SimonSaysButtonController receive these trigger messages and then
 * convert them into states to be passed on to ASimonSaysButtonView instances.
 * 
 * The distinction between trigger and state was mostly made so we could have generic trigger system reusable in other games,
 * independent from these specific buttons, also the two don't have to be in sync as in: the trigger system could sent a 
 * Trigger.Exit messages to an object while the object might ignore it and remain in DisplayState.Activated for example.
 * 
 * The enum has the flags tag because we are using the different states as masks in some of the other components.
 */
[System.Flags]
public enum SimonSaysButtonDisplayState { Default = 1, Hover = 2, Activated = 4, Replay = 8 };