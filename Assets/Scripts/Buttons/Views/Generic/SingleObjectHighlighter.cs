using DG.Tweening;
using InnerDriveStudios.Util;
using UnityEngine;

[Tooltip(@"
/**
 * Button view to color and highlight a single element.
 * This view modifies both the color and emissive color properties of a material.
 */
")]
public class SingleObjectHighlighter : ASimonSaysButtonView
{
	[Tooltip("Color palette to draw base colors from based on a button view's button index.")]
	[SerializeField] private ColorPalette colorPalette;					

	//All fields to change the visuals
	[Tooltip("The MeshRenderer whose main material to modify.")]
	[SerializeField] private MeshRenderer meshRenderer;
	[Tooltip("If useSharedMaterial is false, the MeshRenderer's material will be cloned on Start to avoid changing any other MeshRenderer instances referring to the same material.")]
	[SerializeField] private bool useSharedMaterial = true;
	[Tooltip("Darkens or brightens a material's color property based on the given multiplier.")]
	[SerializeField] private float baseColorModifier = 0.6f;
	[Tooltip("Sets a material's emissive color intensity for the default state.")]
	[SerializeField] private float defaultEmissiveIntensity = 0f;
	[Tooltip("Sets a material's emissive color intensity for the hover state.")]
	[SerializeField] private float hoverEmissiveIntensity = 0.5f;
	[Tooltip("Sets a material's emissive color intensity for the active and replay state.")]
	[SerializeField] private float activatedAndReplayEmissiveIntensity = 1.5f;
	[Tooltip ("Transition time used for non default state changes.")]
	[SerializeField] private float inTransitionTime = 0.1f;
	[Tooltip ("Transition time used for default state change.")]
	[SerializeField] private float outTransitionTime = 0.2f;

	private Color baseColor;				//base color taken from colorpalette multiplied with base color modifier
	private Color interactionColor;			//base color plus (0.1, 0.1, 0.1)
	private Material material;				//the material of the selected node to clone and change the material for
	private int materialPropertyHash;		

	public override void SetButtonIndex(int pButtonIndex)
	{
		baseColor = colorPalette[pButtonIndex] * baseColorModifier;
		interactionColor = colorPalette[pButtonIndex] + new Color(1, 1, 1) * 0.1f;

		//clone the artwork material if needed and set it up for emissive color changes
		material = useSharedMaterial ? meshRenderer.sharedMaterial : meshRenderer.material;
		material.color = baseColor;
		material.EnableKeyword("_EMISSION");
		material.SetEmissiveColor(interactionColor * defaultEmissiveIntensity);
		materialPropertyHash = Shader.PropertyToID("_EmissionColor");
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		DOTween.Kill(material);

		switch (pNewDisplayState)
		{
			case SimonSaysButtonDisplayState.Default:
				material.DOColor(interactionColor * defaultEmissiveIntensity, materialPropertyHash, outTransitionTime);
				break;

			case SimonSaysButtonDisplayState.Hover:
				material.DOColor(interactionColor * hoverEmissiveIntensity, materialPropertyHash, inTransitionTime);
				break;

			case SimonSaysButtonDisplayState.Activated:
			case SimonSaysButtonDisplayState.Replay:
				material.DOColor(interactionColor * activatedAndReplayEmissiveIntensity, materialPropertyHash, inTransitionTime);
				break;
		}
	}

	private void OnDestroy()
	{
		//Clean up after ourselves when needed
		DOTween.Kill(material);
		if (!useSharedMaterial) Destroy(material);
	}

}