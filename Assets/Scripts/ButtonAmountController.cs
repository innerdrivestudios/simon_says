using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using InnerDriveStudios.Util;
using DG.Tweening;
using static DG.Tweening.DOTweenCYInstruction;

[Tooltip(@"
/**
 * This Script controls how many of the button controllers that are a child of the GameController
 * are actually being active at any-one time. In addition, if that amount changes, the ButtonAmountController
 * performs the animations required to show/hide the corresponding buttons.
 */
")]
[DisallowMultipleComponent]
public class ButtonAmountController : MonoBehaviour
{
	[Tooltip("Button amount is communicated from UISlider to this object through a FloatSetting object.")]
	[SerializeField] private FloatSetting buttonAmount;
	[Tooltip("Time it takes to rotate all buttons to their new location based on the requested amount of buttons.")]
	[SerializeField] private float rotationDuration = 0.5f;
	[SerializeField] private Ease rotationEase = Ease.InOutCirc;
	[SerializeField] private float scaleInDuration = 0.5f;
	[SerializeField] private Ease scaleInEase = Ease.InOutCirc;
	[SerializeField] private float scaleOutDuration = 0.5f;
	[SerializeField] private Ease scaleOutEase = Ease.InOutCirc;
	
	private int lastButtonAmount = -1;
	//we don't rotate the buttons, but rotate their parent (pivot)
	private Transform[] buttonPivots;
	private bool busy = false;

	//events indicating the new button amount active
	[SerializeField] private UnityEvent<int> onChangingAmountStart;
	[SerializeField] private UnityEvent<int> onChangingAmountEnd;

	private void Awake()
	{
		buttonPivots = transform.GetChildren();
	}

	private void OnEnable()
	{
		buttonAmount.onIntChanged += UpdateButtonAmount;
	}

	private void OnDisable()
	{
		buttonAmount.onIntChanged -= UpdateButtonAmount;
	}

	private void Start()
	{
		//setup the initial state of the button, which is inactive and scale 0
		foreach (Transform button in buttonPivots)
		{
			//child 0 is the actual button controller, we want to rotate the pivot,
			//but scale the button controller child (otherwise it would scale towards the central hub)
			button.GetChild(0).localScale = Vector3.zero;
			button.gameObject.SetActive(false);
		}
		lastButtonAmount = 0;
	}

	private void UpdateButtonAmount (int pNewButtonAmount)
	{
		pNewButtonAmount = Mathf.Clamp(pNewButtonAmount, 2, buttonPivots.Length);

		if (pNewButtonAmount == lastButtonAmount)
		{
			Debug.Log("Button amount hasn't changed.", this);
			return;
		}

		if (busy)
		{
			Debug.Log("Already busy.", this);
			return;
		}

		busy = true;
		onChangingAmountStart.Invoke(pNewButtonAmount);
		StartCoroutine(UpdateButtonAmountInternal(pNewButtonAmount));
	}

	private IEnumerator UpdateButtonAmountInternal(int pNewButtonAmount)
	{
		if (pNewButtonAmount < lastButtonAmount) yield return HideSurplusButtons(pNewButtonAmount);
		yield return RedistributeSpaceForNewSituation(pNewButtonAmount);
		if (pNewButtonAmount > lastButtonAmount) yield return ShowMissingButtons(pNewButtonAmount);

		lastButtonAmount = pNewButtonAmount;
		busy = false;

		onChangingAmountEnd.Invoke(pNewButtonAmount);
	}

	private IEnumerator HideSurplusButtons(int pNewButtonAmount)
	{
		Sequence sequence = DOTween.Sequence();

		//scale buttons down to 0 with a slight interval
		for (int i = pNewButtonAmount; i < buttonPivots.Length; i++)
		{
			sequence.Insert(0.1f * i, buttonPivots[i].GetChild(0).DOScale(Vector3.zero, scaleOutDuration).SetEase(scaleOutEase));
		}

		//wait until that whole thing is done so we can deactivate buttons we no longer need, if any
		yield return new WaitForCompletion(sequence);
		
		for (int i = pNewButtonAmount; i < buttonPivots.Length; i++)
		{
			buttonPivots[i].gameObject.SetActive(false);
		}
	}

	private IEnumerator RedistributeSpaceForNewSituation(int pNewAmount)
	{
		yield return null;

		//basically get the angle based on the new amount, but adjust the starting angle
		//so that the first element is at (0,0,1) instead of (1,0,0) in case there is an odd amount of buttons
		float angle = 360 / pNewAmount;
		float startAngle = (pNewAmount % 2 == 1) ? 90-angle : 0;

		Sequence sequence = DOTween.Sequence();

		for (int i = 0; i < pNewAmount; i++)
		{
			sequence.Insert(
				0, 
				buttonPivots[i].DOLocalRotate(Vector3.up * (angle * i - startAngle), rotationDuration).SetEase(rotationEase)
			);
		}

		//We could wait for rotation to complete, but it looks better if you don't 
		//yield return new WaitForCompletion (sequence);
	}

	/**
	 * Generate a sequence that activates and scales in any missing buttons from scale 0 to 1.
	 */
	private IEnumerator ShowMissingButtons(int pNewAmount)
	{
		Sequence sequence = DOTween.Sequence();
		Transform buttonPivot;
		Transform buttonController;

		for (int i = lastButtonAmount; i < pNewAmount; i++)
		{
			buttonPivot = buttonPivots[i];
			buttonController = buttonPivot.GetChild(0);
			buttonController.localScale = Vector3.zero;
			buttonPivot.gameObject.SetActive(true);
			sequence.Insert(0.1f * i, buttonController.DOScale(Vector3.one, scaleInDuration).SetEase(scaleInEase));
		}

		yield return new WaitForCompletion(sequence);
	}

}
