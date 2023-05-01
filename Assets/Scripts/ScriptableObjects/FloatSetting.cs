using System;
using UnityEngine;

[Tooltip(@"
/**
 * As the name implies this Scriptable object defines a float value, 
 * that you can pass around, change and use to dispatch changed events to any interested parties.
 * Also the value is automatically restored/saved on Application load/quit (or onChanged in case of WebGL).
 * Load/save mechanism is very rudimentary and based on simple PlayerPrefs values.
 * 
 * Most UI component values (e.g. sliders) are floats, 
 * which allows them to interact directly with ScriptableObjects of this type, 
 * however, since most of our settings in code mostly require discrete integer values, 
 * we also added some intValue wrappers, that modify the underlying float value.
 */
")]
[CreateAssetMenu(menuName = "SimonSays/Float Setting")]
public class FloatSetting : ScriptableObject
{
    //Description is not used, only for our own sanity ;)
    [TextArea(2,5)]
    [SerializeField] private string description;

    public event Action<float> onFloatChanged = delegate { };
    public event Action<int> onIntChanged = delegate { };

    public float floatValue  
    {
        get
        {
            return _value;
        }

        set
        {
            _value = Mathf.Clamp(value, minValue, maxValue);
            checkDispatch();
        }
    }

    public int intValue
    {
        get
        {
            return (int)_value;
        }

        set
        {
            floatValue = value;
        }
    }

	private void checkDispatch()
	{
        if (lastFloatValue != floatValue)
        {
            onFloatChanged.Invoke(floatValue);

            //if the float value didn't change the int value would surely not have changed so we only check this here
            if (lastIntValue != intValue)
            {
                onIntChanged.Invoke(intValue);
            }

            //Since WebGL doesn't automatically save the value on application exit, we save it every time it changes
            if (Application.platform == RuntimePlatform.WebGLPlayer) StoreValue();
        }

        lastFloatValue = floatValue;
        lastIntValue = intValue;	
	}

	[SerializeField] private float _value = 0;
    [SerializeField] private bool resetOnLoad = true;
    [SerializeField] private float defaultValue = 0;

    //these need to be accessible from the outside for the FloatSettingValueMapper
    [field:SerializeField] public float minValue { get; private set; } = 0;
    [field:SerializeField] public float maxValue { get; private set; } = 0;

    private float lastFloatValue = float.MinValue;
    private int lastIntValue = int.MinValue;

	private void OnEnable()
	{
        if (resetOnLoad)
        {
            floatValue = defaultValue;
        }
        else
		{
            RestoreValue();
		}
	}

	private void OnDisable()
	{
        StoreValue();
	}

	private void RestoreValue()
	{
        floatValue = PlayerPrefs.GetFloat(name, defaultValue);
    }

	private void StoreValue()
	{
        PlayerPrefs.SetFloat(name, floatValue);
    }

	private void OnValidate()
    {
        minValue = Mathf.Min(minValue, maxValue);
        maxValue = Mathf.Max(minValue, maxValue);
        defaultValue = Mathf.Clamp(defaultValue, minValue, maxValue);

        floatValue = _value;
    }

    public void ForceDispatch()
	{
        Debug.Log($"Forcing dispatch on {name} with float value:{floatValue} (int value:{intValue})");
        onFloatChanged.Invoke(floatValue);
        onIntChanged.Invoke(intValue);
	}
}
