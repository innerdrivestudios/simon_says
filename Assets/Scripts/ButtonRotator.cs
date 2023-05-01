using UnityEngine;

[Tooltip(@"
/**
 * Takes care of rotating the central hub that the buttons are on.
 */
")]
[DisallowMultipleComponent]
public class ButtonRotator : MonoBehaviour
{
	[SerializeField] private FloatSetting rotationSpeedLevel;
    [SerializeField] private int rotationSpeedMultiplier = 15;
    [SerializeField] private int resetSpeedMultiplier = 10;

	private float rotationSpeed = 0;

	private void OnEnable()
	{
		rotationSpeedLevel.onIntChanged += RotationSpeedLevel_onIntChanged;
	}

	private void OnDisable()
	{
		rotationSpeedLevel.onIntChanged -= RotationSpeedLevel_onIntChanged;
	}

	private void RotationSpeedLevel_onIntChanged(int pNewRotationLevelValue)
	{
		rotationSpeed = pNewRotationLevelValue * rotationSpeedMultiplier;
	}

	void Update()
    {
        if (rotationSpeed > 0)
        {
            transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
        } 
        else
		{
			//reset to central position
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * resetSpeedMultiplier);
		}
    }
}

