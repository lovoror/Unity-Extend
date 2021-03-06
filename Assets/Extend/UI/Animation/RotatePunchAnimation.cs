using System;
using DG.Tweening;
using UnityEngine;

namespace Extend.UI.Animation {
	[Serializable]
	public class RotatePunchAnimation : PunchAnimation {
		protected override Tween DoGenerateTween(Transform t) {
			return t.DOPunchRotation(Punch, Duration, Vibrato, Elasticity).SetDelay(Delay);
		}
	}
}