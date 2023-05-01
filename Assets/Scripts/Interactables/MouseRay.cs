using UnityEngine;

/**
 * Very simple example of how we can control the TriggerManager to implement mouse based controls.
 * Haven't tried adding VR controls yet, maybe someday :).
 */
[RequireComponent(typeof(TriggerManager))]
[DisallowMultipleComponent]
public class MouseRay : MonoBehaviour
{
    private Camera cam;
	private TriggerManager triggerManager;

	private void Awake()
	{
		cam = Camera.main;
		triggerManager = GetComponent<TriggerManager>();
		triggerManager.supportHover = !Application.isMobilePlatform;
	}

	private void Update()
    {
		triggerManager.UpdateRay(
			cam.ScreenPointToRay(Input.mousePosition),
			Input.GetMouseButton(0)
		);
    }
}
