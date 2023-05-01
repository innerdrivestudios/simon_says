using UnityEngine;

[Tooltip(@"
/**
 * Simple ASimonSaysButtonView implementation that plays a sound when the view's display state matches the stateMask.
 * Sound is pitched based on the button index and the pitch offset.
 */
")]
public class AudioPlayer : ASimonSaysButtonView
{
	[Tooltip("The audio source to play when the view enters a display state that matches the provided state mask.")]
	[SerializeField] private AudioSource audioSource;
	[Tooltip("If the view changes to any of the flagged display states in this mask, the audiosource will play.")]
	[SerializeField] private SimonSaysButtonDisplayState stateMask;
	[Tooltip("This number is multiplied with the button index (0, 1, 2, 3, 4) and added to the AudioSource's starting pitch.")]
	[SerializeField] private float pitchOffset = 0.1f;

	public override void SetButtonIndex(int pButtonIndex)
	{
		audioSource.pitch += pButtonIndex * pitchOffset;
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		if (stateMask.HasFlag(pNewDisplayState)) audioSource.Play();
	}
}
