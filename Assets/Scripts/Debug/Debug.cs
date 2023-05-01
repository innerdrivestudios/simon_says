using System.Diagnostics;
using UnityEngine;

/**
 * This static Debug class has the same name and is in the same namespace as the UnityEngine.Debug class,
 * overriding it and delegating to the standard Debug class methods. All delegating methods have been
 * made conditional so all Debug method calls can easily be excluded in a release build.
 * 
 * Note that not every Debug class method has been included, only the ones actually in use by this project,
 * so in a different project you might have to extend this class.
 */
public static class Debug 
{
	[Conditional("DEBUG_ENABLED")]
	public static void Log (object pInfo) {
		UnityEngine.Debug.Log (pInfo);
	}

	[Conditional("DEBUG_ENABLED")]
	public static void LogWarning (object pInfo) {
		UnityEngine.Debug.LogWarning (pInfo);
	}

	[Conditional("DEBUG_ENABLED")]
	public static void Log (object pInfo, Object pContext) {
		UnityEngine.Debug.Log (pInfo, pContext);
	}
}
