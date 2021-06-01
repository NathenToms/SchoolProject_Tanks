using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
	[SerializeField] private GameSettings settings;

	#region Properties

	public float MusicVolume { get { return settings.MusicVolume; } set { settings.MusicVolume = value; } }

	public float EffectVolume { get { return settings.EffectVolume; } set { settings.EffectVolume = value; } }

	public float DialogueVolume { get { return settings.DialogueVolume; } set { settings.DialogueVolume = value; } }

	public bool UseSubtitles { get { return settings.UseSubtitles; } set { settings.UseSubtitles = value; } }

	public WindowMode WindowMode { get { return settings.WindowMode; } set { settings.WindowMode = value; } }

	public Vector2 Resolution { get { return settings.Resolution; } set { settings.Resolution = value; } }

	#endregion

	#region Events

	public event EventHandler MusicVolumeChanged;

	public event EventHandler EffectVolumeChanged;

	public event EventHandler DialogueVolumeChanged;

	public event EventHandler UseSubtitlesChange;

	public event EventHandler WindowModeChange;

	public event EventHandler ResolutionChange;

	#endregion

	#region Public Methods

	/// <summary>
	/// Toggle using subtitles
	/// </summary>
	/// <param name="state">Turn of subtitles</param>
	public void ChangeWindowMode(int modeID)
	{
		//if (modeIndex == 0)
		//{
		//	WindowMode = WindowMode.Fullscreen;
		//}

		//if (modeIndex == 1)
		//{
		//	WindowMode = WindowMode.Borderless;
		//}

		//if (modeIndex == 2)
		//{
		//	WindowMode = WindowMode.Windowed;
		//}
		WindowModeChange(this, EventArgs.Empty);
	}

	/// <summary>
	/// Update the ambient music volume
	/// </summary>
	/// <param name="volume">The new volume</param>
	public void ChangeMusicVolume(float volume)
	{
		float value = Mathf.Clamp(volume, 0, 1);

		MusicVolume = value;
		MusicVolumeChanged(this, EventArgs.Empty);
	}

	/// <summary>
	/// Update the effects volume
	/// </summary>
	/// <param name="volume">The new volume</param>
	public void ChangeEffectVolume(float volume)
	{
		float value = Mathf.Clamp(volume, 0, 1);

		MusicVolume = value;
		MusicVolumeChanged(this, EventArgs.Empty);
	}

	/// <summary>
	/// Update the dialogue volume
	/// </summary>
	/// <param name="volume">The new volume</param>
	public void ChangeDialogueVolume(float volume)
	{
		float value = Mathf.Clamp(volume, 0, 1);

		MusicVolume = value;
		MusicVolumeChanged(this, EventArgs.Empty);
	}

	/// <summary>
	/// Toggle using subtitles
	/// </summary>
	/// <param name="state">Turn of subtitles</param>
	public void ChangeUseSubtitles(bool state)
	{
		UseSubtitles = state;
		UseSubtitlesChange(this, EventArgs.Empty);
	}

	/// <summary>
	/// Change the screen Size
	/// </summary>
	/// <param name="state">new Size</param>
	public void ChangeResolution(int modeID)
	{
		//Resolution = size;
		//ResolutionChange(this, EventArgs.Empty);
	}

	/// <summary>
	/// Change the screen Size
	/// </summary>
	/// <param name="state">new Size</param>
	public void ChangeAntiAliasing(bool state)
	{
		settings.AntiAliasing = state;
	}

	/// <summary>
	/// Change the screen Size
	/// </summary>
	/// <param name="state">new Size</param>
	public void ChangeVSync(bool state)
	{
		settings.VSync = state;
	}

	/// <summary>
	/// Change the screen Size
	/// </summary>
	/// <param name="state">new Size</param>
	public void ChangeShowClouds(bool state)
	{
		settings.ShowClouds = state;
	}

	#endregion
}
