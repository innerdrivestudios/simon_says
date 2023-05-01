using System.Collections;
using UnityEngine;

/**
 * Simple Blinker component which activates/deactivates a GameObject based on a given interval.
 */
public class Blinker : MonoBehaviour
{
	[Tooltip("Which object do you want to blink? Uses 'this' object if left empty.")]
	[SerializeField] private GameObject target;
	[Tooltip("Default interval if you call Blink without a positive parameter.")]
	[Min(0.1f)]
	[SerializeField] private float interval;
	[SerializeField] private bool playOnAwake;
	[SerializeField] private bool stateOnDisable;

	private bool blinking = false;

	private void Awake()
	{
		if (playOnAwake) Blink(interval);

		if (target == gameObject) Debug.LogWarning("Blinker cannot control itself, move this component to a parent GameObject");
	}

	public void Blink (float pInterval = -1)
	{
		if (blinking) return;
		blinking = true;
		StartCoroutine(DoBlink(pInterval > 0 ? pInterval : interval));
	}

	public void StopBlinking(bool pActiveAfter)
	{
		if (!blinking) return;

		blinking = false;
		StopAllCoroutines();
		target.SetActive(pActiveAfter);
	}

	private IEnumerator DoBlink(float pInterval)
	{
		WaitForSeconds wfs = new WaitForSeconds(pInterval);

		while (blinking)
		{
			yield return wfs;
			target.SetActive(!target.activeSelf);
		}
	}

	private void OnDisable()
	{
		StopBlinking(stateOnDisable);
	}


}
