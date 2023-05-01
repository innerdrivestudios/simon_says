using UnityEngine;

public abstract class AbstractAnimator : ScriptableObject
{
	public abstract void Animate(Transform pTarget);
}
