using UnityEngine;

/**
 * All any button view element has to implement are two methods:
 * - accepting a button index to be used in case you need to retrieve button specific information from somewhere
 * - changing the display state
 *
 * We're using an abstract class instead of simply an interface to facilitate correct 
 * serialization in the editor.
 */
public abstract class ASimonSaysButtonView : MonoBehaviour
{
	public abstract void SetButtonIndex(int pButtonIndex);
	public abstract void ChangeDisplayState(SimonSaysButtonDisplayState pNewDisplayState);
}
