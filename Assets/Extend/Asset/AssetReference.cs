using System;
using UnityEngine;
using UnityEngine.Assertions;
using XLua;
using Object = UnityEngine.Object;

namespace Extend.Asset {
	[Serializable, LuaCallCSharp]
	public class AssetReference : IDisposable {
		[SerializeField, HideInInspector]
		private string m_assetGUID;

		public AssetRefObject.AssetStatus AssetStatus => Asset?.Status ?? AssetRefObject.AssetStatus.NONE;
		public bool IsFinished => Asset?.IsFinished ?? false;
		public AssetInstance Asset { get; private set; }

		public AssetReference(AssetInstance instance) {
			Asset = instance;
			Asset?.IncRef();
		}

		public bool GUIDValid => !string.IsNullOrEmpty(m_assetGUID);

		private T GetAsset<T>() where T : Object {
			if( Asset == null ) {
				Asset = AssetService.Get().LoadAssetWithGUID<T>(m_assetGUID);
				Asset.IncRef();
			}

			Assert.AreEqual(Asset.Status, AssetRefObject.AssetStatus.DONE, Asset.Status.ToString());
			return Asset.UnityObject as T;
		}

		public Sprite GetSprite() {
			return GetAsset<Sprite>();
		}

		public Texture GetTexture() {
			return GetAsset<Texture>();
		}

		public Texture3D GetTexture3D() {
			return GetAsset<Texture3D>();
		}

		public TextAsset GetTextAsset() {
			return GetAsset<TextAsset>();
		}

		public Material GetMaterial() {
			return GetAsset<Material>();
		}

		public GameObject GetGameObject() {
			return GetAsset<GameObject>();
		}

		public AudioClip GetAudioClip() {
			return GetAsset<AudioClip>();
		}

		public AnimationClip GetAnimationClip() {
			return GetAsset<AnimationClip>();
		}

		public T GetScriptableObject<T>() where T : ScriptableObject {
			return GetAsset<T>();
		}

		public AssetAsyncLoadHandle LoadAsync(Type typ) {
			var handle = AssetService.Get().LoadAsyncWithGUID(m_assetGUID, typ);
			Assert.IsNotNull(handle.Asset);
			Asset = handle.Asset;
			return handle;
		}

		private GameObject m_go;

		public GameObject Instantiate(Transform parent = null, bool stayWorldPosition = false) {
			if( Asset == null ) {
				Asset = AssetService.Get().LoadAssetWithGUID<GameObject>(m_assetGUID);
				Asset.IncRef();
			}
			if( !( Asset is PrefabAssetInstance prefabAsset ) ) {
				Debug.LogError($"{Asset.AssetPath} is not a prefab!");
				return null;
			}

			return prefabAsset.Instantiate(parent, stayWorldPosition);
		}

		public GameObject Instantiate(Vector3 position, Quaternion quaternion, Transform parent = null) {
			if( Asset == null ) {
				Asset = AssetService.Get().LoadAssetWithGUID<GameObject>(m_assetGUID);
				Asset.IncRef();
			}
			if( !( Asset is PrefabAssetInstance prefabAsset ) ) {
				Debug.LogError($"{Asset.AssetPath} is not a prefab!");
				return null;
			}

			return prefabAsset.Instantiate(position, quaternion, parent);
		}

		public void InitPool(string name, int prefer, int max) {
			if( !( Asset is PrefabAssetInstance prefabAsset ) ) {
				Debug.LogError($"{Asset.AssetPath} is not a prefab!");
				return;
			}
			
			prefabAsset.InitPool(name, prefer, max);
		}

		public override string ToString() {
			return Asset == null || !Asset.UnityObject ? "Not loaded" : Asset.UnityObject.name;
		}

		public void Dispose() {
			if( Asset?.Release() == 0 ) {
				Asset = null;
			}
		}
	}
}