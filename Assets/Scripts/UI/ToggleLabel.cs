using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ToggleLabel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text creditsLabel;

	private bool active = false;
	private bool over = false;

	public UnityEvent onActivated;
	public UnityEvent onDeactivated;
	public UnityEvent onOver;
	public UnityEvent onExit;

	public void OnPointerClick(PointerEventData eventData)
	{
		active = !active;

		if (active) onActivated.Invoke(); else onDeactivated.Invoke();
		creditsLabel.rectTransform.DOScale(Vector3.one * (active ? 1.1f : 1), 0.5f).SetEase(Ease.OutElastic);

		UpdateCreditsLabelColor();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (over) return;

		over = true;
		UpdateCreditsLabelColor();
		onOver.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!over) return;

		over = false;
		UpdateCreditsLabelColor();
		onExit.Invoke();
	}

	private void UpdateCreditsLabelColor()
	{
		//hmm yeah, sorry bout this beauty ;)
		float baseIntensity = over ? (active ? 1 : 0.8f) : (active ? 0.8f : 0);
		creditsLabel.color = new Color(baseIntensity, baseIntensity, baseIntensity, 1);
	}

	//This doesn't trigger any events
	public void Deactivate()
	{
		active = false;
		UpdateCreditsLabelColor();
	}

}
