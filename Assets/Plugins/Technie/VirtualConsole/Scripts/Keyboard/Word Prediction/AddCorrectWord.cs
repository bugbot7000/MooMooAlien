using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technie.VirtualConsole
{
	public class AddCorrectWord : MonoBehaviour
	{
		// Inspector Properties

		public Key key;
		public AutocompleteWordPicker wordPicker;
		public AudioSource confirmSfx;

		// Internal State
		// ..

		private void OnEnable()
		{
			// TODO: Static .keyPressed is a liability waiting to happen - refactor this!
			Key.keyPressed += OnKeyPressed;
		}

		private void OnDisable()
		{
			Key.keyPressed -= OnKeyPressed;
		}

		private void OnKeyPressed(Key srcKey)
		{
			if (srcKey == key)
			{
				WordChosen();
			}
		}

		public void WordChosen()
		{
			wordPicker.ReplaceWord(gameObject.GetComponentInChildren<Text>().text);

			wordPicker.TextField.GetComponent<TextFieldBehaviour>().MoveCaretToEnd();

			confirmSfx.Play();
		}
	}
}