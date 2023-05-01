using UnityEngine;

[Tooltip(@"
/**
 * ParticleSystemColorSetter takes a ColorPalette and ParticleSystem and modifies
 * the startColor of the given ParticleSystem's main module based on the button index
 * into the given ColorPalette.
 */
")]
public class ParticleSystemColorSetter : ASimonSaysButtonView
{
	[Tooltip ("Color palette to draw colors from based on a button view's button index.")]
	[SerializeField] private ColorPalette colorPalette;
	[SerializeField] private ParticleSystem partSystem;

	public override void SetButtonIndex(int pButtonIndex)
	{
		ParticleSystem.MainModule main = partSystem.main;
		main.startColor = colorPalette[pButtonIndex];
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		//do nothing
	}

}
