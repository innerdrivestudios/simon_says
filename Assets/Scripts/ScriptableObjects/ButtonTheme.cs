using UnityEngine;

[Tooltip(@"
/**
 * A button theme allows you to specify a number of prefabs (min of 1 :)),
 * so that it can return a prefab based on a given button index.
 * 
 * In case the button index is higher than the prefab count, the button index simply wraps around, 
 * that way you can use a single button view prefab and everything will still work as planned.
 * 
 * See the ButtonThemes folder for examples.
 */
")]
[CreateAssetMenu(menuName = "SimonSays/Button Theme")]
public class ButtonTheme : ScriptableObject
{
	[SerializeField] private GameObject[] buttonViewPrefabs;

	public GameObject GetViewPrefab(int pButtonIndex)
	{
		return buttonViewPrefabs[pButtonIndex % buttonViewPrefabs.Length];
	}
}
