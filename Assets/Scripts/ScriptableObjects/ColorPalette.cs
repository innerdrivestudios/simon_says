using UnityEngine;

[Tooltip(@"
/**
 * Used by certain button types.
 * Note that you can create any kind of button data container that you want,
 * simply create a prefab for your button that understands your data container 
 * and pull the elements you need from it in the SetButtonIndex method.
 *
 * If you provide less colors than the amount of buttons in the scene, 
 * the color index will simply wrap around using a modulo operator (e.g. colors will be reused).
 */
")]
[CreateAssetMenu(menuName = "SimonSays/Button Color Palette")]
public class ColorPalette : ScriptableObject
{
	[SerializeField] private Color[] colors;

	public int ColorCount => (colors == null)? 0 : colors.Length;
	public Color this[int pColorIndex] => colors[pColorIndex % colors.Length];
	
}
