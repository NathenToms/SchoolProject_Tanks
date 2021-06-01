using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]

public class GameSettings : ScriptableObject
{
	[Range(0, 1)] public float MusicVolume;
	[Range(0, 1)] public float EffectVolume;
	[Range(0, 1)] public float DialogueVolume;

	public float FOV;

	public int SubtitleSize;
	public int FramerateCap;

	public bool UseSubtitles = true;
	public bool ShowClouds = true;
	public bool VSync = true;
	public bool AntiAliasing = true;

	public WindowMode WindowMode;

	[HideInInspector] public Vector2 Resolution;

	// TODO -- Subtitle Type
}

public enum WindowMode
{
	Fullscreen,
	Borderless,
	Windowed
}

[CustomEditor(typeof(GameSettings))]

public class GameSettingsEditor : Editor
{
	private GameSettings _gameSettings;
	private int _selected = 0;

	private List<string> _resolutions = new List<string>
	{
		"1920,1080",
		"1680,1050",
		"1600,1024",
		"1600,900",
		"1440,900",
		"1366,768",
	};

	private void OnEnable()
	{
		_gameSettings = (GameSettings)target;
		_selected = ParseResolution(_gameSettings.Resolution);
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		int lastSelected = _selected;

		_selected = EditorGUILayout.Popup("Resolutions", _selected, _resolutions.ToArray());

		if (lastSelected != _selected)
		{
			string[] newResolutions = _resolutions[_selected].Split(',');

			int x = int.Parse(newResolutions[0]);
			int y = int.Parse(newResolutions[1]);

			_gameSettings.Resolution = new Vector2(x, y);
		}
	}

	private int ParseResolution(Vector2 resolution)
	{
		string newResolution = resolution.x + "," + resolution.y;
		return _resolutions.IndexOf(newResolution);
	}
}