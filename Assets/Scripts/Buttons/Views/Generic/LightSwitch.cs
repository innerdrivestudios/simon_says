using System;
using UnityEngine;

[Tooltip(@"
/**
 * LightSwitch component which allows you to specify a ColorPalette and a bunch of LightStates.
 * LightStates specify the intensity to use for a specific display state  (a 0 or missing value turns the light off for that state).
 * The ColorPalette is used to get/set the color of the light based on the view's button index.
 */
")]
public class LightSwitch : ASimonSaysButtonView
{
	[Serializable]
	private struct LightState
	{
		public SimonSaysButtonDisplayState displayState;
		public float intensity;
	}

	[SerializeField] private ColorPalette colorPalette;
	[SerializeField] private Light lightToEnable;
	[SerializeField] private LightState[] lightStates;

	private Color baseColor;

	public override void SetButtonIndex(int pButtonIndex)
	{
		baseColor = colorPalette[pButtonIndex];
		lightToEnable.color = baseColor;
		lightToEnable.enabled = false;
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		bool matchingLightStateFound = false;

		foreach (LightState lightState in lightStates)
		{
			if (lightState.displayState.HasFlag(pNewDisplayState))
			{
				lightToEnable.intensity = lightState.intensity;
				matchingLightStateFound = true;
				break;
			}
		}

		lightToEnable.enabled = matchingLightStateFound;
	}
}
