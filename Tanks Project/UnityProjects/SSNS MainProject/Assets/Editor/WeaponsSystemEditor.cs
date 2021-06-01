using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponsSystem))]
public class WeaponsSystemEditor : ExtendedEditor
{
	[HideInInspector] public bool[] showWeapons = new bool[0];

	// EditorStyles.toolbarButton
	WeaponsSystem _weaponsSystem;

	GUIStyle iconStyle = new GUIStyle();

	private void OnEnable()
	{
		_weaponsSystem = (WeaponsSystem)target;
		iconStyle.normal.background = _weaponsSystem.defaultTexture;
		showWeapons = new bool[] { false, false, false, false, false };
	}

	bool _showDependencies = true;
	bool _showControlls = false;

	public override void OnInspectorGUI()
	{
		if (_weaponsSystem.useDefaultEditor)
		{
			base.OnInspectorGUI();
		}
		else
		{
			GUILayout.Space(10);

			EditorGUILayout.BeginVertical("box");

			GUILayout.Label("Gun Controller", EditorStyles.boldLabel);

			DrawSettingEditor(_weaponsSystem.GunController, ref _weaponsSystem.showWeaponsSystem);

			EditorGUILayout.EndVertical();
		}

		serializedObject.ApplyModifiedProperties();

		DrawFootnote("Use Default Editor", ref _weaponsSystem.useDefaultEditor);
	}

	private void DrawWeaponUI(int index, GUIStyle style)
	{
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Weapon", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
		{
			showWeapons[index] = !showWeapons[index];
		}

		EditorGUILayout.EndHorizontal();

		if (showWeapons[index])
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical("box");

			GUILayout.Box("", style, GUILayout.Width(50), GUILayout.Height(50));

			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical("box");

			int int1 = 0;

			EditorGUILayout.IntField(int1);
			EditorGUILayout.IntField(int1);
			EditorGUILayout.IntField(int1);

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();
	}

	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i)
		{
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}
}