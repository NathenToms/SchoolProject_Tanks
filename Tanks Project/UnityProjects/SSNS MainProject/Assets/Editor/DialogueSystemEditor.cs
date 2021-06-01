using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueSystem))]
public class DialogueSystemEditor : ExtendedEditor
{
	private DialogueSystem _dialogueSystem;
	private DialogueSystemWindow _window;

	private void OnEnable()
	{
		_dialogueSystem = (DialogueSystem)target;
	}

	public override void OnInspectorGUI()
	{
		GUILayout.Space(10);

		if (_dialogueSystem.ShowDefaultEditor)
		{
			DrawDefaultInspector();
		}
		else
		{
			if (GUILayout.Button("Open Editor"))
			{
				_window = EditorWindow.GetWindow<DialogueSystemWindow>();
				_window.SetDialogueSystem(_dialogueSystem);
			}
		}
		
		DrawFootnote("This is a Custom Dialogue System Editor", ref _dialogueSystem.ShowDefaultEditor);
	}
}
