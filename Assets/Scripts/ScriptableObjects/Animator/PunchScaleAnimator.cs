using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "SimonSays/Punch Scale Animator")]
public class PunchScaleAnimator : AbstractAnimator
{
	public float strength = 2;
	public float duration = 1;
	public int vibrato = 10;
	public float elasticity = 1; 

	public override void Animate(Transform pTarget)
	{
		DOTween.Kill(pTarget);
		pTarget.localScale = Vector3.one;
		pTarget.DOPunchScale(Vector3.one * strength, duration, vibrato, elasticity);
	}
}
