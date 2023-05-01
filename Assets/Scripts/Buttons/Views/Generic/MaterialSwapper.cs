using UnityEngine;

[Tooltip(@"
/**
 * Setup for MaterialSwapper view. 
 * Not used at the moment.
 */
")]
public class MaterialSwapper : ASimonSaysButtonView
{
    [SerializeField] private int materialIndex = 0;
    [SerializeField] private Material alternativeMaterial;
	[SerializeField] private SimonSaysButtonDisplayState materialMask;

	[SerializeField] private MeshRenderer meshRenderer;
    private Material originalMaterial;

	public override void SetButtonIndex(int pButtonIndex)
	{
		originalMaterial = meshRenderer.sharedMaterials[materialIndex];
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		if (materialMask.HasFlag(pNewDisplayState))
		{
			meshRenderer.sharedMaterials[materialIndex] = alternativeMaterial; 
		}
		else
		{
			meshRenderer.sharedMaterials[materialIndex] = originalMaterial;
		}

		meshRenderer.sharedMaterials = meshRenderer.sharedMaterials;
	}
}
