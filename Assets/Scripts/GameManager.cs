using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InnerDriveStudios.Util;

[Tooltip(@"
/**
 * GameManager is responsible for running the main game loop of generating and replaying sequences.
 */
")]
[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
	[SerializeField] private FloatSetting buttonAmount;
	[SerializeField] private SimonSaysButtonController buttonControllerPrefab;
	[SerializeField] private float buttonDistanceFromCenter;
	[SerializeField] private int forcedFrameRate = -1;

	[Tooltip("Only for testing purposes")] [SerializeField] private bool autoStart = false;

	[Tooltip(
		"The FloatSettingValueMapper takes an input value and maps it to a different range. "+
		"In this case: replay-speed-settings to actual delays."
	)]
	[SerializeField] private FloatSettingValueMapper replaySpeed2DelayValueMapper;
	
	[Header ("Events")]

	[Tooltip("Dispatched when the gamemanager has been initialized")]
	public UnityEvent onGameInit;
	[Tooltip("Dispatched when a new game is started")]
	public UnityEvent onGameStart;
	[Tooltip("Dispatched before a pattern is replayed")]
	public UnityEvent onPatternReplay;      
	[Tooltip("Dispatched before a pattern has to be repeated")]
	public UnityEvent onPatternRepeat;      
	[Tooltip("Dispatched after a pattern has been succesfully repeated")]
	public UnityEvent onPatternRepeated;
	[Tooltip("Dispatched after a pattern has not been succesfully repeated")]
	public UnityEvent onPatternFail;
	[Tooltip("Dispatched when the current game ends (triggers after onPatternFail with small delay)")]
	public UnityEvent onGameEnd;			

	//reference to all the buttoncontrollers, no matter whether they are active or not
	private SimonSaysButtonController[] allButtons;
	//reference to only the buttoncontrollers that are currenlty ACTIVE (during gameplay)
	private SimonSaysButtonController[] activeButtons;

	//reference to a list of buttons we need to repeat, might contain duplicates
	private List<SimonSaysButtonController> pattern = new List<SimonSaysButtonController>();

	//keeps track of whether we are currently/still in play
	private bool inPlay = false;											

	//keeps track of any last clicked button during the interactive replay
	//to see if the player chose the correct button
	private SimonSaysButtonController lastClickedButton = null;

	//cached waitforseconds instances
	private WaitForSeconds replayWaitForSeconds;
	private WaitForSeconds oneSecondPause;
	private WaitForSeconds halfASecondPause;

	private void Awake()
	{
		if (forcedFrameRate > 0) Application.targetFrameRate = forcedFrameRate;

		//Make sure we have instantiate the maximum amount of possible buttons
		for (int i = 0; i < buttonAmount.maxValue; i++)
		{
			GameObject controllerParent = new GameObject("ControllerPivot");
			controllerParent.SetActive(false);
			controllerParent.transform.parent = transform;
			SimonSaysButtonController ssbc = Instantiate(buttonControllerPrefab, new Vector3(buttonDistanceFromCenter, 0, 0), Quaternion.identity, controllerParent.transform);
		}
	}

	private void Start()
	{
		oneSecondPause = new WaitForSeconds(1);
		halfASecondPause = new WaitForSeconds(0.5f);

		//get all buttons at this point in time no matter whether they are active so we can disable interactivity on all of them when needed
		allButtons = GetComponentsInChildren<SimonSaysButtonController>(true);
		SetButtonsInteractable(allButtons, true);

		onGameInit.Invoke();
		if (autoStart) StartGame();
	}

	private void SetButtonsInteractable(IEnumerable<SimonSaysButtonController> pButtons, bool pInteractable)
	{
		foreach (SimonSaysButtonController button in pButtons)
		{
			button.ForceState(SimonSaysButtonDisplayState.Default);
			button.SetInteractable(pInteractable);
		}
	}

	[ContextMenu("Start Game")]
	public void StartGame()
	{
		if (inPlay)
		{
			Debug.Log("Cannot start a game that hasn't ended yet.");
			return;
		}

		StartCoroutine(GameLoopCoroutine());
	}

	private IEnumerator GameLoopCoroutine()
	{
		pattern.Clear();
		replayWaitForSeconds = new WaitForSeconds(replaySpeed2DelayValueMapper.GetValue());
		SetButtonsInteractable(allButtons, false);
		
		//set this every time the game starts since amount of active buttons might have been changed
		activeButtons = GetComponentsInChildren<SimonSaysButtonController>(false);

		//and pick one of the buttons randomly as the first element
		pattern.Add(activeButtons.GetRandomElement());

		//update our state and inform any listeners
		inPlay = true;
		onGameStart.Invoke();

		yield return oneSecondPause;

		while (inPlay)
		{
			yield return oneSecondPause;
			yield return AutoReplayCurrentPattern();
			yield return halfASecondPause;
			yield return InteractivePatternRepeat();

			if (!inPlay) break;

			onPatternRepeated.Invoke();

			//make it one step harder...
			pattern.Add(activeButtons.GetRandomElement());
		}

		inPlay = false;
		SetButtonsInteractable(allButtons, true);
		onPatternFail.Invoke();
		yield return oneSecondPause;
		onGameEnd.Invoke();
	}

	IEnumerator AutoReplayCurrentPattern()
	{
		onPatternReplay.Invoke();

		//replay (highlight) every button for a short amount of time
		foreach (SimonSaysButtonController button in pattern)
		{
			yield return replayWaitForSeconds;
			button.ForceState(SimonSaysButtonDisplayState.Replay);
			yield return replayWaitForSeconds;
			button.ForceState(SimonSaysButtonDisplayState.Default);
		}
	}

	IEnumerator InteractivePatternRepeat()
	{
		onPatternRepeat.Invoke();
		
		//Make sure everything is set up to listen to button clicks correctly
		lastClickedButton = null;
		SimonSaysButtonController.OnTriggered -= SimonSaysButton_OnTriggered;
		SimonSaysButtonController.OnTriggered += SimonSaysButton_OnTriggered;
		SetButtonsInteractable(activeButtons, true);
		
		int index = 0;
		
		foreach (SimonSaysButtonController button in pattern)
		{
			//when a user clicks a button an event will be triggered, which set the last clicked button
			yield return new WaitUntil(IsButtonSet);

			//expected button clicked?
			if (lastClickedButton == pattern[index])
			{
				//move to next button
				index++;
				//and clear lastClickedButton, so we can wait for another one
				lastClickedButton = null;
			}
			else //whoopsie...
			{
				inPlay = false;
				break;
			}
		}

		SetButtonsInteractable(activeButtons, false);
		SimonSaysButtonController.OnTriggered -= SimonSaysButton_OnTriggered;
	}

	private bool IsButtonSet ()
	{
		return lastClickedButton != null;
	}

	private void SimonSaysButton_OnTriggered(SimonSaysButtonController pSource)
	{
		lastClickedButton = pSource;
	}

	private void OnApplicationQuit()
	{
		//Make sure we don't have any event listeners remaining behind when we exit play (mode)
		SimonSaysButtonController.OnTriggered -= SimonSaysButton_OnTriggered;
	}
}
