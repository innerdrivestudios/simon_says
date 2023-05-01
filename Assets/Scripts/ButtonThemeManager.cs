using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using DG.Tweening;
using static DG.Tweening.DOTweenCYInstruction;

[Tooltip(@"
/**
 * Takes care of installing different prefabs (with ASimonSaysButtonView components)
 * on to the button controllers that are children of the GameManager hub.
 */
")]
[DisallowMultipleComponent]
public class ButtonThemeManager : MonoBehaviour
{
	//A ButtonTheme can return the prefab that needs to be instantiated to get a view.
	//The ButtonThemeManager keeps track of a list of different themes to choose from...
    [SerializeField] private ButtonTheme[] themes;
	//... based on the themeSelectionSetting's value
    [SerializeField] private FloatSetting themeSelectionSetting;

	[Header("Easing settings")]
	[SerializeField] private Ease outEase = Ease.Linear;
	[SerializeField] private float outTime = 0.5f;
	[SerializeField] private Ease inEase = Ease.OutBack;
	[SerializeField] private float inTime = 0.5f;
	[SerializeField] private float staggerDelay = 0.2f;

	//Events to trigger as we are changing themes so we can enable/disable interface elements etc
	[SerializeField] private UnityEvent<ButtonTheme> onChangingThemeStart;
	[SerializeField] private UnityEvent<ButtonTheme> onChangingThemeEnd;

	//References all existing ButtonControllers so we can ask them to install a new view
	private SimonSaysButtonController[] buttonControllers;

	private int lastThemeIndex = -1;
	private bool busy = false;

	void Awake()
	{
		//Get ALL button controllers, no matter whether they are active or not
		buttonControllers = GetComponentsInChildren<SimonSaysButtonController>(true);
		Debug.Log($"Found {buttonControllers.Length} buttoncontrollers...");
	}

	private void OnEnable()
	{
		themeSelectionSetting.onIntChanged += UpdateTheme;
	}

	private void OnDisable()
	{
		themeSelectionSetting.onIntChanged -= UpdateTheme;
	}

	private void UpdateTheme(int pThemeIndex)
	{
		//Not doing this everywhere, was a bit too lazy, but still it's good practice ;)
		Assert.IsNotNull(themes, "Themes cannot be null");
		Assert.IsFalse(themes.Length == 0, "Themes cannot be empty");
		Assert.IsTrue(pThemeIndex > -1 && pThemeIndex < themes.Length, "Invalid index provided");

		if (pThemeIndex == lastThemeIndex || busy) return;

		StartCoroutine(UpdateThemeInternal(pThemeIndex));
	}

	private IEnumerator UpdateThemeInternal(int pThemeIndex)
	{
		busy = true;
		ButtonTheme theme = themes[pThemeIndex];
		onChangingThemeStart.Invoke(theme);

		bool firstTime = lastThemeIndex == -1;

		if (firstTime)
		{
			//Don't do any animation the first time, just make sure buttons are there
			for (int i = 0; i < buttonControllers.Length; i++)
			{
				InitializeView(theme, i);
			}
		}
		else
		{
			//Run a sequence with a bunch of minisequences to replace the buttonviews
			Sequence sequence = DOTween.Sequence();
			for (int i = 0; i < buttonControllers.Length; i++)
			{
				//we need to capture this variable in a local variable otherwise it will use the wrong i value 
				int index = i;
				Sequence buttonSequence = DOTween.Sequence();
				//buttonSequence.Append(buttonControllers[i].transform.DOLocalRotate(Vector3.right * 180, 0.5f).SetEase(Ease.Linear));
				buttonSequence.Append(buttonControllers[i].transform.DOScale(Vector3.zero, outTime).SetEase(outEase));
				//generates a closure which is better prevented, but since it happens so few and far between, I didn't optimize this
				//Solution would be to create your own wrapper class and cache 6 instances of it, to be reused over and over again
				buttonSequence.AppendCallback(() => InitializeView(theme, index));
				//buttonSequence.Append(buttonControllers[i].transform.DOLocalRotate(Vector3.right * 360, 0.5f).SetEase(Ease.OutQuad));
				buttonSequence.Append(buttonControllers[i].transform.DOScale(Vector3.one, inTime).SetEase(inEase));
				sequence.Insert(i * staggerDelay, buttonSequence);
			}

			yield return new WaitForCompletion(sequence);
		}

		lastThemeIndex = pThemeIndex;
		busy = false;
		onChangingThemeEnd.Invoke(theme);
	}

	private void InitializeView (ButtonTheme theme, int pIndex)
	{
		//tell a specific button controller to instantiate a new prefab and set the prefab it's button index
		//up to the controller to cache/pool this instantiation or not (spoiler alert: currently it doesn't)
		buttonControllers[pIndex].InitializeNewView(
			theme.GetViewPrefab(pIndex), pIndex
		);
	}

}
