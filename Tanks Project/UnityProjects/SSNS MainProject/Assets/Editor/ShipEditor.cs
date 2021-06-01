using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ShipController))]
public class ShipEditor : ExtendedEditor
{
	// EditorStyles.toolbarButton
	ShipController _shipController;

	void Block(String name, Action action)
	{
		EditorGUILayout.BeginVertical("box");
		GUILayout.Label(name, EditorStyles.boldLabel);
		action.Invoke();
		EditorGUILayout.EndVertical();
	}

	private void OnEnable()
	{
		_shipController = (ShipController)target;
	}

	bool _showDependencies = true;
	bool _showControlls = false;
	bool _showAuto = false;

	public override void OnInspectorGUI()
	{
		if (_shipController.useDefaultEditor)
		{
			base.OnInspectorGUI();
		}
		else
		{
			GUILayout.Space(10);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Controlls", EditorStyles.toolbarButton))
			{
				_showAuto = false;
				_showDependencies = false;
				_showControlls = true;
			}
			if (GUILayout.Button("Dependencies", EditorStyles.toolbarButton))
			{
				_showAuto = false;
				_showDependencies = true;
				_showControlls = false;
			}
			if (GUILayout.Button("Auto", EditorStyles.toolbarButton))
			{
				_showAuto = true;
				_showDependencies = false;
				_showControlls = false;
			}
			EditorGUILayout.EndHorizontal();

			if (_showDependencies)
			{
				Block("Cameras", () => {
					DrawField("pilotCamera");
					DrawField("gunnerCamera");
				});

				Block("Transforms", () =>  {
					DrawField("shipModel");
					GUILayout.Space(5);
					DrawField("gunnerPivot");
					DrawField("gunnerObject");
					GUILayout.Space(5);
					DrawField("gunnerslockUI");
				});

				Block("Audio", () => {
					DrawField("shootingSoundController");
				});

				Block("Warp Effect", () => {
					DrawField("warpEffect1");
					DrawField("warpEffect2");
				});
			}

			if (_showAuto)
			{
				Block("Auto", () => {
					DrawField("shieldProjector");
					DrawField("weaponsSystem");
					DrawField("playerHUD");
					DrawField("rigidbody");
					DrawField("shipHP");
				});

				Block("Lock On Target", () => {
					DrawField("lockOnTarget");
				});
			}

			if (_showControlls)
			{
				Block("Control Options", () => {
				DrawField("InvertedControls");
				DrawField("UnlimitedBoost");
				DrawField("AimAssist");
				});

				Block("Properties", () => {
					DrawField("myProperties");
					DrawSettingEditor(_shipController.Properties, ref _shipController.showStats);
				});

				Block("Behaviour", () => {
					DrawField("myBehaviour");
					DrawSettingEditor(_shipController.Behaviour, ref _shipController.showBehaviour);
				});
			}

			serializedObject.ApplyModifiedProperties();
		}

		DrawFootnote("Use Default Editor", ref _shipController.useDefaultEditor);
	}
}