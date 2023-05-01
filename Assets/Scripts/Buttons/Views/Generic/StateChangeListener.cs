using UnityEngine;
using UnityEngine.Events;

[Tooltip(@"
/**
 * Generic view which fires UnityEvents based on whether the current view state matches or not.
 */
")]
public class StateChangeListener : ASimonSaysButtonView
{
	[Tooltip("States to watch out for")]
	[SerializeField] private SimonSaysButtonDisplayState stateMask;

	//Fired when the stateMask is matched
	public UnityEvent onStateMatch;
	//Fired when the stateMask is not matched
	public UnityEvent onStateMismatch;

	public override void SetButtonIndex(int pButtonIndex)
	{
		//ChangeDisplayState(SimonSaysButtonDisplayState.Default);
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		if (stateMask.HasFlag(pNewDisplayState))
		{
			onStateMatch.Invoke();
		}
		else
		{
			onStateMismatch.Invoke();
		}
	}
}
