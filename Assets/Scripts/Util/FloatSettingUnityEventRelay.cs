using UnityEngine;
using UnityEngine.Events;

[Tooltip(@"
/**
 * FloatSettingUnityEventRelay implements an adapter/mediator of sorts between FloatSetting objects
 * (which is a ScriptableObject and cannot have UnityEvents) and Scene objects that would like to 
 * listen to changes to a FloatSetting object.
 */
")]
public class FloatSettingUnityEventRelay : MonoBehaviour
{
	[SerializeField] private FloatSetting floatSetting;
	[SerializeField] UnityEvent<float> onFloatChanged;
	[SerializeField] UnityEvent<int> onIntChanged;

	private void OnEnable()
	{
		if (floatSetting != null)
		{
			floatSetting.onFloatChanged += onFloatChanged.Invoke;
			floatSetting.onIntChanged += onIntChanged.Invoke;
		}

		//Make sure any listeners are at least updated once, needed since any updates
		//don't propagate to the UI elements when it is disabled for some reason
		floatSetting.ForceDispatch();
	}

	private void OnDisable()
	{
		if (floatSetting != null)
		{
			floatSetting.onFloatChanged -= onFloatChanged.Invoke;
			floatSetting.onIntChanged -= onIntChanged.Invoke;
		}
	}
}
