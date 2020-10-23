﻿using Extend.Common.Editor.InspectorGUI;
using UnityEngine;
using UnityEditor;


namespace Extend.UI.Scroll.Editor {
	[CustomEditor(typeof(LoopScrollRect), true)]
	public class LoopScrollRectInspector : ExtendInspector {
		private int index;
		private float speed = 1000;

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			EditorGUILayout.Space();

			var scroll = (LoopScrollRect)target;
			GUI.enabled = Application.isPlaying;

			EditorGUILayout.BeginHorizontal();
			if( GUILayout.Button("Clear") ) {
				scroll.ClearCells();
			}

			if( GUILayout.Button("Refresh") ) {
				scroll.RefreshCells();
			}

			if( GUILayout.Button("Refill") ) {
				scroll.RefillCells();
			}

			if( GUILayout.Button("RefillFromEnd") ) {
				scroll.RefillCellsFromEnd();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUIUtility.labelWidth = 45;
			float w = ( EditorGUIUtility.currentViewWidth - 100 ) / 2;
			EditorGUILayout.BeginHorizontal();
			index = EditorGUILayout.IntField("Index", index, GUILayout.Width(w));
			speed = EditorGUILayout.FloatField("Speed", speed, GUILayout.Width(w));
			if( GUILayout.Button("Scroll", GUILayout.Width(45)) ) {
				scroll.ScrollToCell(index, speed);
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}