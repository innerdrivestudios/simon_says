using UnityEngine;
using TMPro;
using UnityEngine.Events;

[Tooltip(@"
/**
 * Adapter that allows you to pass float and int values from events to textmeshpro components.
 */
")]
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPMeshProAdapter : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI label;
	[SerializeField] private string prefix;
	[SerializeField] private int digits = 0;

	public UnityEvent onValueUpdated;

	private void Reset()
	{
		label = GetComponent<TextMeshProUGUI>();
	}

	private void Awake()
	{
		//label.text = "";
	}

	public void SetValue(float pValue)
	{
		string value = pValue.ToString("F" + digits.ToString());
		label.text = string.IsNullOrEmpty(prefix) ? (value) : (prefix.Replace("\\t", "\t") + value);
		onValueUpdated.Invoke();
	}

	public void SetValue(int pValue)
	{
		string value = ""+pValue;
		label.text = string.IsNullOrEmpty(prefix) ? (value) : (prefix.Replace("\\t", "\t") + value);
		onValueUpdated.Invoke();
	}

}
