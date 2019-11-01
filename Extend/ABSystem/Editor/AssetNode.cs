using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ABSystem.Editor {
	public class AssetNode {
		private string Path => importer.assetPath;

		private string AssetName {
			get {
				var assetName = System.IO.Path.GetDirectoryName( importer.assetPath ) + "/" + System.IO.Path.GetFileNameWithoutExtension( importer.assetPath );
				assetName = assetName.Replace( '\\', '/' );
				return assetName;
				// return System.IO.Path.GetFileNameWithoutExtension( importer.assetPath );
			}	
		}

		public string AssetBundleName {
			get => importer.assetBundleName;
			private set => importer.assetBundleName = value;
		}

		private readonly List<AssetNode> referenceNodes = new List<AssetNode>();
		private readonly AssetImporter importer;

		public AssetNode(string path) {
			importer = AssetImporter.GetAtPath( path );
		}

		public bool IsValid => importer != null;

		private void AddReferenceNode(AssetNode node) {
			if( referenceNodes.Contains( node ) || node == this ) {
				return;
			}

			referenceNodes.Add( node );
		}

		public void BuildRelation() {
			var dependencies = AssetDatabase.GetDependencies( Path );
			foreach( var filePath in dependencies ) {
				var dependencyNode = BuildAssetRelation.GetNode( filePath );
				dependencyNode?.AddReferenceNode( this );
			}
		}

		private static void DeepFirstSearch(AssetNode node, ICollection<AssetNode> collector) {
			if( collector.Contains( node ) ) {
				return;
			}
			collector.Add( node );
			foreach( var referenceNode in node.referenceNodes ) {
				DeepFirstSearch( referenceNode, collector );
			}
		}

		private readonly List<AssetNode> collector = new List<AssetNode>();
		public void RemoveShorterLink() {
			collector.Clear();
			foreach( var node in referenceNodes.SelectMany( referenceNode => referenceNode.referenceNodes ) ) {
				DeepFirstSearch( node, collector );
			}

			foreach( var index in collector.Select( node => referenceNodes.IndexOf( node ) ).Where( index => index >= 0 ) ) {
				referenceNodes.RemoveAt( index );
			}
		}

		public string BuildGraphviz() {
			var sb = new StringBuilder();
			foreach( var node in referenceNodes ) {
				sb.AppendLine( $"{AssetName} -> {node.AssetName}" );
			}

			return sb.ToString();
		}

		private bool OuterLink {
			get {
				if( System.IO.Path.GetExtension( Path ) == ".prefab" )
					return false;

				if( importer is TextureImporter textureImporter ) {
					if( textureImporter.textureType == TextureImporterType.Sprite ) {
						return false;
					}
				}

				return string.IsNullOrEmpty( AssetBundleName );
			}
		}

		public void CalculateABName() {
			if( !string.IsNullOrEmpty( AssetBundleName ) )
				return;

			if( OuterLink ) {
				if( referenceNodes.Count > 0 ) {
					var abName = "";
					foreach( var referenceNode in referenceNodes ) {
						referenceNode.CalculateABName();
						if( string.IsNullOrEmpty( abName ) ) {
							abName = referenceNode.AssetBundleName;
						}
						else if( abName != referenceNode.AssetBundleName ) {
							abName = AssetName;
							break;
						}
					}
					
					AssetBundleName = abName;
					return;
				}
			}
			AssetBundleName = AssetName;
		}

		public int ReferenceCount => referenceNodes.Count;
	}
}