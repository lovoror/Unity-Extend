﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ListView;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Extend.UI.Editor {
	public class UIViewTreeItem : TreeViewItem {
		public UIViewConfiguration.Configuration Configuration { get; }

		public UIViewTreeItem(UIViewConfiguration.Configuration configuration) : base(configuration.GetHashCode()) {
			Configuration = configuration;
		}
	}

	public class UIViewDelegate : IListViewDelegate<UIViewTreeItem> {
		private readonly UIViewConfiguration configurationContext;
		private readonly SerializedObject serializedObject;
		private int selectedIndex = -1;

		public UIViewDelegate(UIViewConfiguration context) {
			configurationContext = context;
			serializedObject = new SerializedObject(context);
		}

		public MultiColumnHeader Header => new MultiColumnHeader(new MultiColumnHeaderState(new[] {
			new MultiColumnHeaderState.Column {headerContent = new GUIContent("Index"), width = 5},
			new MultiColumnHeaderState.Column {headerContent = new GUIContent("Name"), width = 10},
			new MultiColumnHeaderState.Column {headerContent = new GUIContent("UI View"), width = 20},
			new MultiColumnHeaderState.Column {headerContent = new GUIContent("Background Fx"), width = 10},
			new MultiColumnHeaderState.Column {headerContent = new GUIContent("Full Screen"), width = 5},
			new MultiColumnHeaderState.Column {headerContent = new GUIContent("Attach Layer"), width = 10},
			new MultiColumnHeaderState.Column {headerContent = new GUIContent("Transition"), width = 10}
		}));

		public List<TreeViewItem> GetData() {
			return configurationContext.Configurations.Select(configuration => new UIViewTreeItem(configuration)).Cast<TreeViewItem>().ToList();
		}

		public List<TreeViewItem> GetSortedData(int columnIndex, bool isAscending) {
			throw new NotImplementedException();
		}

		private static readonly string[] columnIndexToFieldName = {"Name", "UIView", "BackgroundFx", "FullScreen", "AttachLayer", "Transition"};

		public void Draw(Rect rect, int columnIndex, UIViewTreeItem data, bool selected) {
			var index = Array.IndexOf(configurationContext.Configurations, data.Configuration);
			if( selected ) {
				selectedIndex = index;
			}
			
			if( columnIndex == 0 ) {
				EditorGUI.LabelField(rect, index.ToString());
				return;
			}
			columnIndex--;
			var configurations = serializedObject.FindProperty("configurations");
			var element = configurations.GetArrayElementAtIndex(index);
			var prop = element.FindPropertyRelative(columnIndexToFieldName[columnIndex]);
			
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(rect, prop, GUIContent.none);
			if( EditorGUI.EndChangeCheck() ) {
				serializedObject.ApplyModifiedProperties();
			}
		}

		public void OnItemClick(int id) {
		}

		public void OnContextClick() {
		}

		public void Add() {
			var configurations = serializedObject.FindProperty("configurations");
			configurations.InsertArrayElementAtIndex(configurations.arraySize);
			serializedObject.ApplyModifiedProperties();
		}

		public void Remove() {
			var configurations = serializedObject.FindProperty("configurations");
			if( selectedIndex >= 0 && selectedIndex < configurations.arraySize ) {
				configurations.DeleteArrayElementAtIndex(selectedIndex);
			}
			selectedIndex = -1;
		}

		public void Save() {
			EditorUtility.SetDirty(configurationContext);
			AssetDatabase.SaveAssets();
		}
	}

	public class UIViewEditorWindow : EditorWindow {
		private static UIViewEditorWindow window;
		private static ListView<UIViewTreeItem> listView;

		private UIViewDelegate _delegate;
		private bool refreshFlag;
		private bool dirtyFlag;

		[MenuItem("Window/UIView Window")]
		private static void OpenWindow() {
			if( window ) {
				window.Close();
				window = null;
				return;
			}

			window = GetWindow<UIViewEditorWindow>();
			window.Show();
		}

		private void OnEnable() {
			const string path = "Assets/Resources/" + UIViewConfiguration.FILE_PATH + ".asset";
			var uiViewConfiguration = AssetDatabase.LoadAssetAtPath<UIViewConfiguration>(path);
			if( !uiViewConfiguration ) {
				uiViewConfiguration = CreateInstance<UIViewConfiguration>();
				AssetDatabase.CreateAsset(uiViewConfiguration, path);
			}

			_delegate = new UIViewDelegate(uiViewConfiguration);
			listView = new ListView<UIViewTreeItem>(_delegate);
			listView.Refresh();
		}

		private void OnGUI() {
			ButtonsGUI();
			var controlRect = EditorGUILayout.GetControlRect(
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true));
			if( refreshFlag ) {
				listView?.Refresh();
			}

			listView?.OnGUI(controlRect);
			if( dirtyFlag || refreshFlag ) {
				_delegate.Save();
			}

			dirtyFlag = false;
			refreshFlag = false;
		}

		private void ButtonsGUI() {
			GUILayout.BeginHorizontal();
			if( GUILayout.Button("Add") ) {
				_delegate.Add();
				refreshFlag = true;
			}

			if( GUILayout.Button("Remove") ) {
				_delegate.Remove();
				refreshFlag = true;
			}

			if( GUILayout.Button("Refresh") ) {
				refreshFlag = true;
			}
			
			if( GUILayout.Button("Save") ) {
				_delegate.Save();
			}

			GUILayout.EndHorizontal();
		}
	}
}