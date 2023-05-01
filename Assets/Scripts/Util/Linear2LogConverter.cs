using UnityEngine;
using UnityEngine.Audio;

public class Linear2LogConverter : MonoBehaviour
{
	public string param;
	public AudioMixer mixer;

	public void SetInput(float pInputValue)
	{
		mixer.SetFloat(param, Linear2Log(pInputValue));
	}

	private float Linear2Log(float pInputValue)
	{
		//Log10 returns the e in the formula pInputValue = 10^e
		//For example: 100 = 10^2 so Log10(100) would return 2
		//And similarly:
		//Log10(10) == 1
		//Log10(1) == 0
		//Log10(0.1) == -1
		//To map 0 to 1 to -80 to 0 db for the audiomixer, we first make sure we are not actually mapping 0..1 but 0,0001..1
		//since Log10(0,0001) - Log10(1) is -4 .. 0 and then we multiply with 20 to get -80 .. 0
		return 20.0f * Mathf.Log10(Mathf.Clamp(pInputValue, 0.0001f, 1));
	}
}
