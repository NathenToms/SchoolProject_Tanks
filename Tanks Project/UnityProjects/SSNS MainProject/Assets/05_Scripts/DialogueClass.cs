using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

/// <summary>
/// Used to store the sound clip and text and keep the data connected
/// </summary>
[System.Serializable]
public class DialogueClass
{
	public OwnerType OwnerType = OwnerType.NONE;

	public AudioClip SoundClip;

	public string Name;

	[TextArea(7, 1)] public string[] Text;
    public float[] displayTime;

	public int Index;

	public float volumeMultiplier = 1;

	public int[] alternatives;
}
