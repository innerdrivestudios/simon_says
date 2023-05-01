using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[Tooltip(@"
/**
 * Simple script we can add to textfields on the credits panel.
 */
")]
[RequireComponent(typeof(TMP_Text))]
[DisallowMultipleComponent]
public class LinkClicker : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	[Tooltip("The URL to visit when clicked.")]
    [SerializeField] private string link;
	
	private TMP_Text text;

	private void Awake()
	{
		text = GetComponent<TMP_Text>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!string.IsNullOrEmpty(link))  Application.OpenURL(link);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		//FontStyle is an enum, and conveniently a bitflag one,
		//which means we can perform bit arithmetic on it
		//For example, the line below says, set every bit in the current fontStyle to true,
		//if it was already true or equals the Underline bit
		text.fontStyle |= FontStyles.Underline;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		//Similarly to OnPointerEnter, ~ first generates the complement of Underline, 
		//which is a number in which EVERY bit is set EXCEPT underline, 
		//and then we AND those bits which any bit already set, effectively clearing the Underline bit
		text.fontStyle &= ~FontStyles.Underline;
	}
}
