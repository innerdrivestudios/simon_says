using UnityEngine;

[Tooltip(@"
/**
 * Button view for moving an element up or down based on the view state:
 * Default/Hover -> normal, Activated/Replay -> down.
 * 
 * If animation is also required have a look at TweenedSingleObjectMover.
 */
")]
public class SingleObjectMover : ASimonSaysButtonView
{
	[SerializeField] private Transform objectToMove;
	[SerializeField] private float pressOffset = -0.05f;
	private Vector3 originalPosition;

	public override void SetButtonIndex(int pButtonIndex)
	{
		originalPosition = objectToMove.localPosition;
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		switch (pNewDisplayState)
		{
			case SimonSaysButtonDisplayState.Default:
			case SimonSaysButtonDisplayState.Hover:
				objectToMove.localPosition = originalPosition;
				break;

			case SimonSaysButtonDisplayState.Activated:
			case SimonSaysButtonDisplayState.Replay:
				objectToMove.localPosition = originalPosition + Vector3.up * pressOffset;
				break;
		}
	}

}