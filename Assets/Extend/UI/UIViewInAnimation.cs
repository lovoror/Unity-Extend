using System;
using DG.Tweening;
using Extend.Common;
using Extend.UI.Animation;
using UnityEngine;

namespace Extend.UI {
	[Serializable]
	public class UIViewInAnimation : IUIAnimationPreview {
		public enum AnimationMode {
			STATE,
			ANIMATOR
		}

		public AnimationMode Mode;

		[SerializeField]
		private bool enabled;

		public bool Enabled => enabled;

		[SerializeField]
		private ViewInStateCombine state;

		[SerializeField]
		private AnimatorParamProcessor processor;

		public Tween[] Active(Transform t) {
			switch( Mode ) {
				case AnimationMode.STATE:
					state.Active(t);
					return state.AllTween;
				case AnimationMode.ANIMATOR:
					processor.Apply();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return null;
		}

		public void CacheStartValue(Transform t) {
			if( Mode == AnimationMode.STATE ) {
				state.CacheStartValue(t);
			}
		}

		public Tween[] CollectPreviewTween(Transform transform) {
			return Active(transform);
		}

		public void Editor_Recovery(Transform transform) {
			if( Mode == AnimationMode.STATE ) {
				state.Editor_Recovery(transform);
			}
		}
	}
}