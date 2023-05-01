using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

[Tooltip(@"
/**
 * UISwitcher panel helps enabling the correct child of the main glass panel ui.
 * Due to the UI structure this is a little bit convoluted:
 * 1. we need to move the MaskedUI up and down
 * 2. when the MaskedUI is down we need to deactivate certain children and active others
 * 3. then we need to move the maskedUI up again to a position determined by the child in question
 */
")]
[DisallowMultipleComponent]
public class UISwitcher : MonoBehaviour
{
	//Helper class to hold the data for a rect (just an offset)
	[Serializable]
	private class UIPanel
	{
		public RectTransform rect;
		public float offset;
	}

	[Tooltip(
		"A list of all the UIPanels we would like to be able to enable."+
		" This consists of a reference to a panel and an offset (height) at which the panel should be displayed."
	)]
	[SerializeField] private UIPanel[] uiPanels;

	[Tooltip(
		"The masked panel that contains the UIPanels we want to show/hide. "+
		"This is the panel that we will move down first, before toggling the UIPanel we want to activate, and then move up again."
	)]
	[SerializeField] private RectTransform maskedUI;

	[Tooltip("Which panel has to be shown on application startup? This HAS to be one of the panels in the previously provided list.")]
	[SerializeField] private int defaultPanel;

	[Tooltip("At which offset is the whole masked UI hidden?")]
	[SerializeField] private float uiDownPosition = -100;

	[SerializeField] private float transitionTime = 1;
	[SerializeField] private Ease outPanelEase = Ease.Linear;
	[SerializeField] private Ease inPanelEase = Ease.Linear;
	[SerializeField] private bool ignoreSameState = true;
	
	//We need to reference more globally then I'd like to be able to queue callbacks referring back to these values
	private int currentPanelIndex = -1;
	private UIPanel currentPanel = null;
	private int newPanelIndex = -1;
	private UIPanel newPanel = null;

	private bool canSwitchUsingKeys = Application.isEditor;

	//Triggers event passing in the new panel index and RectTransform
	public UnityEvent<int, RectTransform> onPanelChangeStart;
	public UnityEvent<int, RectTransform> onPanelContentsSwapped;
	public UnityEvent<int, RectTransform> onPanelChangeCompleted;

	private void Awake()
	{
		//make sure we can find all the data for each uipanel quickly
		foreach (UIPanel uiPanel in uiPanels)
		{
			uiPanel.rect.gameObject.SetActive(false);
		}

		ShowPanel(defaultPanel);

		//we use the update loop to check for key input, so we enable this script if we canSwitchUsingKeys
		enabled = canSwitchUsingKeys;
	}

	public void ShowPanel (int pNewPanelIndex)
	{
		if (pNewPanelIndex == currentPanelIndex && ignoreSameState) return;

		DOTween.Kill(maskedUI);

		newPanelIndex = pNewPanelIndex;
		newPanel = null;

		if (newPanelIndex > -1)
		{
			newPanel = uiPanels[newPanelIndex];

			if (newPanel == null)
			{
				Debug.LogWarning("Request panel has not been set up correctly.");
				return;
			}
		}

		onPanelChangeStart.Invoke(newPanelIndex, newPanel?.rect);

		Sequence sequence = DOTween.Sequence(maskedUI);

		//is there a current ui that needs to be phased out?
		if (currentPanel == null)
		{
			//if not, just jump to down position
			maskedUI.anchoredPosition = new Vector2(0, uiDownPosition);
			sequence.AppendCallback(SwapPanelContents);
		} 
		else
		{
			//first move panel all the way down if the newPanel demands it
			if (currentPanel?.rect != newPanel?.rect)
			{
				sequence.Append(maskedUI.DOAnchorPosY(uiDownPosition, transitionTime).SetEase(outPanelEase));
				sequence.AppendCallback(SwapPanelContents);
			}
		}

		if (newPanel != null)
		{
			//move the newly swapped in panel to its preferred position
			sequence.Append(maskedUI.DOAnchorPosY(newPanel.offset, transitionTime).SetEase(inPanelEase));
		}

		sequence.AppendCallback(SwapPanelCompleted);
	}

	private void SwapPanelContents()
	{
		if (currentPanel != null) currentPanel.rect.gameObject.SetActive(false);
		if (newPanel != null) newPanel.rect.gameObject.SetActive(true);
		
		onPanelContentsSwapped.Invoke(newPanelIndex, newPanel?.rect);
	}

	private void SwapPanelCompleted()
	{
		onPanelChangeCompleted.Invoke(newPanelIndex, newPanel?.rect);

		//Finally set newpanel as current panel and clear newpanel values
		currentPanelIndex = newPanelIndex;
		currentPanel = newPanel;
		newPanelIndex = -1;
		newPanel = null;
	}

	//Only for debugging!

	KeyCode[] keysToCheck = { KeyCode.Alpha1, KeyCode.Alpha2 , KeyCode.Alpha3 , KeyCode.Alpha4 };

	private void Update()
	{
		if (!Input.anyKeyDown) return;

		for (int i = 0; i < Mathf.Min (keysToCheck.Length, uiPanels.Length); i++)
		{
			if (Input.GetKeyDown(keysToCheck[i]))
			{
				ShowPanel(i);
				break;
			}
		}
	}
}
