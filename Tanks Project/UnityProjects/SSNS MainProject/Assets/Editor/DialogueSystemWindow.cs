using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SNSSTypes;

public enum SortType { ID, alphabetical, owner }

public class DialogueSystemWindow : ExtendedEditorWindow
{
	private DialogueSystem _dialogueSystem;

	private SortType _sortType;

	private Vector2 _clipListScrollPos;
	private Vector2 _clipScrollPos;

	private int _currentIndex = -1;

	private bool _showAdd = false;
	private bool _showCurrent = true;

	private void OnEnable()
	{
		titleContent.text = "Dialogue System Window";
	}

	private void OnGUI()
	{
		Block("Dialogue", () =>
		{
			SerializedObject dialogueSystem = new SerializedObject(_dialogueSystem);

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(dialogueSystem.FindProperty("audioSource"), true);
			EditorGUILayout.PropertyField(dialogueSystem.FindProperty("dialogueText"), true);

			EditorGUILayout.EndHorizontal();

			Block("Options", () =>
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();

				GUILayout.Label("Sort Type ", GUILayout.Width(65));

				SortType oldSortType = _sortType;
				_sortType = (SortType)EditorGUILayout.EnumPopup(_sortType);

				if (oldSortType != _sortType)
				{
					ReSort();
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			});

			Block("Dialogue Clips", () =>
			{

				if (_dialogueSystem == null) return;

				if (_dialogueSystem.Dialogue == null)
				{
					_dialogueSystem.Dialogue = new List<DialogueClass>();
				}


				SerializedProperty dialogueArray = dialogueSystem.FindProperty("Dialogue");

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.BeginVertical("box", GUILayout.Width(350));

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Edit", EditorStyles.toolbarButton))
				{
					_showAdd = false;
					_showCurrent = true;
					GUI.FocusControl(null);
				}

				if (GUILayout.Button("Add", EditorStyles.toolbarButton))
				{
					_showAdd = true;
					_showCurrent = false;
					GUI.FocusControl(null);
				}
				EditorGUILayout.EndHorizontal();

				if (_showAdd)
				{
					Block("Add Clip", () =>
					{

						_clipScrollPos = EditorGUILayout.BeginScrollView(_clipScrollPos);

						EditorGUILayout.PropertyField(dialogueSystem.FindProperty("NewDialogue").FindPropertyRelative("Name"), true);
						EditorGUILayout.PropertyField(dialogueSystem.FindProperty("NewDialogue").FindPropertyRelative("Text"), true);
						GUILayout.Space(5);
						EditorGUILayout.PropertyField(dialogueSystem.FindProperty("NewDialogue").FindPropertyRelative("SoundClip"), true);
						GUILayout.Space(5);
						EditorGUILayout.PropertyField(dialogueSystem.FindProperty("NewDialogue").FindPropertyRelative("OwnerType"), true);
						EditorGUILayout.PropertyField(dialogueSystem.FindProperty("NewDialogue").FindPropertyRelative("volumeMultiplier"), true);
						GUILayout.Space(5);
						EditorGUILayout.PropertyField(dialogueSystem.FindProperty("NewDialogue").FindPropertyRelative("alternatives"), true);

						if (GUILayout.Button("Add New Clip", EditorStyles.toolbarButton))
						{
							_dialogueSystem.GenerateNewDialogue();
							ReSort();
						}

						EditorGUILayout.EndScrollView();
					});
				}

				if (_showCurrent)
				{
					Block("Current Clip", () =>
					{
						if (_currentIndex >= 0 && _currentIndex < dialogueArray.arraySize)
						{
							if (_currentIndex >= 0)
							{
								Block("", () =>
								{
									_clipScrollPos = EditorGUILayout.BeginScrollView(_clipScrollPos);

									EditorGUILayout.PropertyField(dialogueArray.GetArrayElementAtIndex(_currentIndex).FindPropertyRelative("Name"), true);
									GUILayout.Space(5);
									EditorGUILayout.PropertyField(dialogueArray.GetArrayElementAtIndex(_currentIndex).FindPropertyRelative("Text"), true);
									GUILayout.Space(5);
									EditorGUILayout.PropertyField(dialogueArray.GetArrayElementAtIndex(_currentIndex).FindPropertyRelative("OwnerType"), true);
									EditorGUILayout.PropertyField(dialogueArray.GetArrayElementAtIndex(_currentIndex).FindPropertyRelative("volumeMultiplier"), true);
									GUILayout.Space(5);
									EditorGUILayout.PropertyField(dialogueArray.GetArrayElementAtIndex(_currentIndex).FindPropertyRelative("SoundClip"), true);
									GUILayout.Space(5);
									EditorGUILayout.PropertyField(dialogueArray.GetArrayElementAtIndex(_currentIndex).FindPropertyRelative("alternatives"), true);

									Block("Auto", () =>
									{
										EditorGUILayout.IntField("Dialogue Index", _dialogueSystem.Dialogue[_currentIndex].Index);
										if (_dialogueSystem.Dialogue[_currentIndex].SoundClip != null)
										{
											EditorGUILayout.FloatField("Sound Clip Length", _dialogueSystem.Dialogue[_currentIndex].SoundClip.length);
										}
									});

									EditorGUILayout.EndScrollView();
								});
							}
						}
					});
				}

				EditorGUILayout.EndVertical();

				if (_dialogueSystem.Dialogue.Count > 0)
				{
					EditorGUILayout.BeginVertical("box");

					_clipListScrollPos = EditorGUILayout.BeginScrollView(_clipListScrollPos);

					for (int index = 0; index < dialogueArray.arraySize; index++)
					{
						OwnerType owner = (OwnerType)dialogueArray.GetArrayElementAtIndex(index).FindPropertyRelative("OwnerType").enumValueIndex;
						string clipName = dialogueArray.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue;
						int clipIndex = dialogueArray.GetArrayElementAtIndex(index).FindPropertyRelative("Index").intValue;
						string duration = _dialogueSystem.Dialogue[index].SoundClip == null ? "0.00" : _dialogueSystem.Dialogue[index].SoundClip.length.ToString("F2");
						string buttonName = clipIndex + " | " + owner + " | " + (clipName == "" ? "unnamed" : clipName) + " | " + duration;

						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button(buttonName, GUILayout.Height(30)))
						{
							GUI.FocusControl(null);
							_currentIndex = index;
						}

						if (GUILayout.Button(_dialogueSystem.SoundIcon, GUILayout.Width(30), GUILayout.Height(30)))
						{
							_dialogueSystem.PlayQuickClip(_dialogueSystem.Dialogue[index].SoundClip);
							GUI.FocusControl(null);
							_currentIndex = index;
						}
						EditorGUILayout.EndHorizontal();
					}

					EditorGUILayout.EndScrollView();

					EditorGUILayout.EndVertical();
				}

				EditorGUILayout.EndHorizontal();

				dialogueSystem.ApplyModifiedProperties();

			});
		});
	}

	private void ReSort()
	{
		if (_sortType == SortType.alphabetical)
		{
			_dialogueSystem.Dialogue.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));
		}

		if (_sortType == SortType.ID)
		{
			_dialogueSystem.Dialogue.Sort((p1, p2) => p1.Index.CompareTo(p2.Index));
		}

		if (_sortType == SortType.owner)
		{
			_dialogueSystem.Dialogue.Sort((p1, p2) => p1.OwnerType.CompareTo(p2.OwnerType));
		}
	}

	public void SetDialogueSystem(DialogueSystem newDialogueSystem)
	{
		_dialogueSystem = newDialogueSystem;
		_dialogueSystem.NewDialogue = new DialogueClass();
	}
}
