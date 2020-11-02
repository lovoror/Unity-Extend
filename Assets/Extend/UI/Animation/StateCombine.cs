using System;

namespace Extend.UI.Animation {
	[Serializable]
	public class StateCombine : StateAnimationTemplate<MoveStateAnimation, RotateStateAnimation, ScaleStateAnimation, FadeStateAnimation> {
	}
	
	[Serializable]
	public class ViewInStateCombine : StateAnimationTemplate<MoveInAnimation, RotateInAnimation, ScaleInAnimation, FadeInAnimation> {
	}
	
	[Serializable]
	public class ViewOutStateCombine : StateAnimationTemplate<MoveOutAnimation, RotateOutAnimation, ScaleOutAnimation, FadeOutAnimation> {
	}
	
	[Serializable]
	public class ViewLoopStateCombine : StateAnimationTemplate<MoveLoopAnimation, RotateLoopAnimation, ScaleLoopAnimation, FadeLoopAnimation> {
	}

}