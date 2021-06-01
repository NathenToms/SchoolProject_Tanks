using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
	[SerializeField] private Image button;

	public Sprite DefaultSprite;
	public Sprite SelectedSprite;

	private float _defaultWidth;
	private float _defaultHeight;

	private void OnEnable()
	{
		_defaultWidth = GetComponent<RectTransform>().sizeDelta.x;
		_defaultHeight = GetComponent<RectTransform>().sizeDelta.y;

		button.sprite = DefaultSprite;
	}

	// When you hover this button
	public void OnEnter() { }

	// When your mouse leaves this button
	public void OnExit() { }

	// When you click on this button
	public void OnSelect()
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(_defaultWidth * 1.1f, _defaultHeight);
		button.sprite = SelectedSprite;
	}

	// When you click on a different  button
	public void OnDeselect()
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(_defaultWidth, _defaultHeight);
		button.sprite = DefaultSprite;
	}
}
