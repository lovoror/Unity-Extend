using DG.Tweening;
using Extend.Asset;
using Extend.Common;
using Extend.DebugUtil;
using Extend.LuaUtil;
using Extend.Network;
using Extend.UI.i18n;
using UnityEngine;

namespace Extend {
	internal static class StaticServiceInitializer {
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnInit() {
			DOTween.Init(true, true, LogBehaviour.Default);

			CSharpServiceManager.Initialize();
			CSharpServiceManager.Register(new GlobalCoroutineRunnerService());
			CSharpServiceManager.Register(new StatService());
			CSharpServiceManager.Register(new ErrorLogToFile());
			CSharpServiceManager.Register(new AssetService());
			CSharpServiceManager.Register(new GameSystem());
			CSharpServiceManager.Register(new SpriteAssetService());
			CSharpServiceManager.Register(new I18nService());
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnSceneLoaded() {
			CSharpServiceManager.Register(new LuaVM());
			CSharpServiceManager.Register(new TickService());
			CSharpServiceManager.Register(new NetworkService());

			var mode = GameSystem.Get().SystemSetting.GetString("GAME", "Mode");
			if( mode != "Shipping" ) {
				using( var assetRef = AssetService.Get().Load("Console", typeof(GameObject)) ) {
					var go = assetRef.Instantiate();
					CSharpServiceManager.Register(go.GetComponent<InGameConsole>());
				}
			}
			Application.targetFrameRate = 60;
		}
	}
}