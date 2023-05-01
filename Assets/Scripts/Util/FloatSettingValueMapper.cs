using System;
using UnityEngine;

/**
 * Allows you to reinpret a FloatSetting to a remapped value.
 */
[Serializable]
public struct FloatSettingValueMapper 
{
	[Tooltip("The FloatSetting to read values from (floatValue, min and max inputValues)")][SerializeField] private FloatSetting input;
	[Tooltip("The minimum value of the output range")][SerializeField] private float outMin;
	[Tooltip("The maximum value of the output range")][SerializeField] private float outMax;

	public float GetValue ()
	{
		float inValue = input.floatValue;
		inValue = Math.Max(input.minValue, Math.Min(inValue, input.maxValue));
		return (inValue - input.minValue) * (outMax - outMin) / (input.maxValue - input.minValue) + outMin;
	}
}
