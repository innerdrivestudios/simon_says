using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSyncSetter : MonoBehaviour
{

	//https://docs.unity3d.com/ScriptReference/QualitySettings-vSyncCount.html
	public void SetVSync (int pLevel)
	{
		QualitySettings.vSyncCount = pLevel;
	}
}
