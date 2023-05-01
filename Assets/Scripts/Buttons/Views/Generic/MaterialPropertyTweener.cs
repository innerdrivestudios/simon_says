using UnityEngine;
using DG.Tweening;

[Tooltip(@"
/**
 * A view which tweens a single property on a given meshrenderer's material based on 
 * whether a specific display state is matched. Future enhancement would be to have different
 * values per state instead of only matched and unmatched values, but for now it will have to do ;).
 */
")]
public class MaterialPropertyTweener : ASimonSaysButtonView
{
	[Tooltip("Which MeshRenderer's material should be modified?")]
	[SerializeField] private MeshRenderer meshRenderer;
	[Tooltip("If useSharedMaterial is false, the MeshRenderer's material will be cloned on Start to avoid changing any other MeshRenderer instances referring to the same material.")]
	[SerializeField] private bool useSharedMaterial = true;
	[Tooltip("Which material property should be tweened?")]
	[SerializeField] private string property;
	[Tooltip("If the current display state matches any of the states in this mask, the given property will tween to the given match value in the given match duration.")]
	[SerializeField] private SimonSaysButtonDisplayState stateMask;
	[Tooltip("The value for the given material property if the current display state does match the provided state mask.")]
	[SerializeField] private float matchedValue = 0.5f;
	[Tooltip("How long it takes the property to reach the matched value?")]
	[SerializeField] private float matchedDuration = 0.5f;
	[Tooltip ("The value for the given material property if the current display state does not match the provided state mask.")]
	[SerializeField] private float unmatchedValue = 0.5f;
	[Tooltip("How long it takes the property to reach the unmatched value?")]
	[SerializeField] private float unmatchedDuration = 0.5f;

    private Material material;
	private int propertyID;

	public override void SetButtonIndex(int pButtonIndex)
	{
		//Decide whether to use a material clone or not
		material = useSharedMaterial ? meshRenderer.sharedMaterial : meshRenderer.material;
		propertyID = Shader.PropertyToID(property);
		material.DOFloat(unmatchedValue, propertyID, 0);
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		DOTween.Kill(material);

		if (stateMask.HasFlag(pNewDisplayState))
		{
			material.DOFloat(matchedValue, propertyID, matchedDuration);
		}
		else
		{
			material.DOFloat(unmatchedValue, propertyID, unmatchedDuration);
		}
	}

	private void OnDestroy()
	{
		DOTween.Kill(material);

		//make sure to clean up after ourselves if we created a new material on start
		if (!useSharedMaterial) Destroy(material);
	}
}
