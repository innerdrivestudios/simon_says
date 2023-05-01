using UnityEngine;

/**
 * Simple on screen debug class used during development to figure some things out ;).
 */
public class InfoPrinter : MonoBehaviour
{
    public TMPro.TMP_Text infoField;

    // Start is called before the first frame update
    void Start()
    {
        infoField.text = Application.platform + "\n"+Input.touchSupported+"\n"+Input.mousePresent +"\n" + Application.isMobilePlatform;
    }
}