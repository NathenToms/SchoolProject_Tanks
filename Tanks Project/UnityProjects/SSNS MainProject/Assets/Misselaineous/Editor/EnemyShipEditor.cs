using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController), true)]
public class EnemyShipEditor : Editor
{
	EnemyController controller;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (controller.Stats)
		{
			GUILayout.Space(5f);
			DrawSettingEditor(controller.myStats, ref controller.showStats);
			GUILayout.Space(5f);
			EditorGUILayout.HelpBox("This is a custone editor located in the 'EnemyShipEditor' script", MessageType.Info);
		}
	}

	void DrawSettingEditor(Object settings, ref bool foldout)
	{
		foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
		if (foldout)
		{
			Editor editor = CreateEditor(settings);
			if (editor) { editor.OnInspectorGUI(); }
		}
	}

	private void OnEnable()
	{
		controller = (EnemyController)target;
	}
}
