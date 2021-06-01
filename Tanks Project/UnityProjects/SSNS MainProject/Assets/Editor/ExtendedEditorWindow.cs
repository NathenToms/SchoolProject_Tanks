using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExtendedEditorWindow : EditorWindow
{
	protected SerializedObject serializedObject;
	protected SerializedProperty currentProperty;

	protected SerializedProperty selectedProperty;
	private string selectedPropertyPath;

	private int lastTab = -1;

	protected void DrawProperties(SerializedProperty prop, bool drawChildren)
	{
		string lastPropPath = string.Empty;
		foreach (SerializedProperty p in prop)
		{
			if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
			{
				EditorGUILayout.BeginHorizontal();
				p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
				EditorGUILayout.EndHorizontal();

				if (p.isExpanded)
				{
					EditorGUI.indentLevel++;
					DrawProperties(p, drawChildren);
					EditorGUI.indentLevel--;
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
				lastPropPath = p.propertyPath;
				EditorGUILayout.PropertyField(p, drawChildren);
			}
		}
	}

	protected void Block(string name, Action action)
	{
		EditorGUILayout.BeginVertical("box");
		if (name != "") GUILayout.Label(name, EditorStyles.boldLabel);
		action.Invoke();
		EditorGUILayout.EndVertical();
	}

	protected int  DrawSidebar(SerializedProperty prop)
	{
		int counter = 0;
		foreach (SerializedProperty p in prop)
		{
			if (GUILayout.Button(p.displayName)) {
				GUI.FocusControl(null);
				lastTab = counter;
				selectedPropertyPath = p.propertyPath;
			}
			counter++;
		}

		if (!string.IsNullOrEmpty(selectedPropertyPath)) {
			selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
		}

		return lastTab;
	}

	protected void DrawSettingEditor(UnityEngine.Object settings, ref bool foldout)
	{
		foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
		if (foldout)
		{
			Editor editor = Editor.CreateEditor(settings);
			if (editor) { editor.OnInspectorGUI(); }
		}
	}

	protected void DrawField(string propName, bool relative)
	{
		if (relative && currentProperty != null)
		{
			EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propName), true);

		}
		else if (serializedObject != null)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
		}
	}

	protected void Apply()
	{
		serializedObject.ApplyModifiedProperties();
	}
}
