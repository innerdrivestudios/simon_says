using DG.Tweening;
using UnityEngine;

[Tooltip(@"
/**
 * View specific for a SpaceInvaders Mesh, which causes the view to slowly move up and down
 * and rotate around the y axis when not hovering, but rotate towards the camera when hovering instead.
 */
")]

public class FluidMotionView : ASimonSaysButtonView
{
	[Tooltip ("Additional phase offset per button to make sure all meshes are not moving up and down at the same time.")]
	[SerializeField] private float phaseOffsetPerIndex = 1.256f;
	[Tooltip("How many units the mesh moves up and down over the y axis relative to the starting point.")]
	[SerializeField] private float moveRange = 0.1f;
	[Tooltip("Movement is done using a sine wave based on accumulated time using += Time.deltaTime * moveSpeed.")]
	[SerializeField] private float moveSpeed = 2;
	[Tooltip("Rotation is done using a sine wave based on accumulated time using += Time.deltaTime * rotationSpeed.")]
	[SerializeField] private float rotationSpeed = 1;
	[Tooltip("Mesh is rotated by the rotation sine result * rotationRange each frame.")]
	[SerializeField] private float rotationRange = 1;
	
	private float phaseOffset = 0;
	private float moveTime = 0;			//cumulative move time
	private float rotationTime = 0;     //cumulative rotation time
	private Vector3 startPosition;

	private SimonSaysButtonDisplayState displayState = SimonSaysButtonDisplayState.Default;

	public override void SetButtonIndex(int pButtonIndex)
	{
		phaseOffset = phaseOffsetPerIndex * pButtonIndex;
		startPosition = transform.localPosition;
	}

	public override void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState)
	{
		displayState = pNewDisplayState;
		if (pNewDisplayState == SimonSaysButtonDisplayState.Activated || 
			pNewDisplayState == SimonSaysButtonDisplayState.Replay)
		{
			//give it a spin over time
			transform.DOBlendableLocalRotateBy(new Vector3(0, 360, 0), 1, RotateMode.LocalAxisAdd);
		}
	}

	private void Update()
	{
		moveTime += Time.deltaTime * moveSpeed;
		rotationTime += Time.deltaTime * rotationSpeed;

		Vector3 targetPosition = startPosition + Vector3.up * Mathf.Sin(phaseOffset + moveTime) * moveRange;
		transform.localPosition = targetPosition;

		//if we are not hovering or activating just rotate mesh around y axis
		if (displayState != SimonSaysButtonDisplayState.Hover && 
			displayState != SimonSaysButtonDisplayState.Activated)
		{

			transform.Rotate(0, Mathf.Sin(phaseOffset + rotationTime) * rotationRange * Time.deltaTime, 0);
		}
		//otherwise if no tweening is currently happening, rotate over the y axis to face the camera
		else if (!DOTween.IsTweening(transform))
		{
			Vector3 forward = transform.position - Camera.main.transform.position;
			forward.y = 0;
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 3);
		}
	}
}
