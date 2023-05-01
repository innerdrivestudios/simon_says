using UnityEngine;
using DG.Tweening;

[Tooltip(@"
/**
 * Button view for animating an element up or down based on the current view state.
 * Default/Hover -> UP, Activated/Replay -> DOWN
 */
")]
public class TweenedSingleObjectMover : ASimonSaysButtonView
{
	[SerializeField] private Transform objectToMove;
	[SerializeField] private float pressOffset = -0.05f;

	[SerializeField] private float pressDuration = 0.1f;
	[SerializeField] private Ease pressEase = Ease.Linear;
	[SerializeField] private float releaseDuration = 0.1f;
	[SerializeField] private Ease releaseEase = Ease.Linear;

	private Vector3 originalPosition; 

	public override void SetButtonIndex(int pButtonIndex)
	{
		originalPosition = objectToMove.localPosition;
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		DOTween.Kill(objectToMove);

		switch (pNewDisplayState)
		{
			case SimonSaysButtonDisplayState.Default:
			case SimonSaysButtonDisplayState.Hover:
				objectToMove.DOLocalMoveY(originalPosition.y, releaseDuration).SetEase(releaseEase);
				break;

			case SimonSaysButtonDisplayState.Activated:
			case SimonSaysButtonDisplayState.Replay:
				objectToMove.DOLocalMoveY(originalPosition.y + pressOffset, pressDuration).SetEase(pressEase);
				break;
		}
	}

}