using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technie.VirtualConsole
{
	public class VirtualKeyboard : MonoBehaviour
	{
		// Events and callbacks

		public delegate void KeyboardFinishedEvent(bool wasOk, string enteredText);
		public KeyboardFinishedEvent OnKeyboardFinished;

		// Inspector Properties

		public GameObject leftHandTip;
		public GameObject rightHandTip;

		public GameObject leftDrumstick;
		public GameObject rightDrumstick;

		public VirtualInputField typedStringDisplay;

		public Key doneKey;
		public ShiftKeyBehaviour shiftKey;

		public bool showDebug;

		// Internal State

		void Start ()
		{
			typedStringDisplay.Select();
			typedStringDisplay.ActivateInputField();
		}
		
		void OnEnable()
		{
			Key.keyPressed += OnDonePressed;
		}

		void OnDisable()
		{
			Key.keyPressed -= OnDonePressed;
		}

		public void SetText(string txt)
		{
			typedStringDisplay.text = txt;
		}

		private void OnDonePressed(Key srcKey)
		{
			if (srcKey == doneKey)
			{
				Console.output.Log("VirtualKeyboard - user finished entering text!");
				
				if (OnKeyboardFinished != null)
					OnKeyboardFinished(true, typedStringDisplay.text);
			}
		}

		public void OnKeyPressed(string newChar)
		{
			if (newChar == null || newChar.Length == 0)
				return;

			if (shiftKey.IsShiftActive)
				newChar = newChar.ToUpper();
			else
				newChar = newChar.ToLower();

			char ch = newChar[0];

			// Explicitly remap this glyph into a newline character
			// Note we can't use KeyCode.Return as the text field doesn't interpret this as a newline
			if (ch == '↵')
				ch = '\n';

			typedStringDisplay.DoAppend(ch);
			typedStringDisplay.ForceLabelUpdate();
		}

		public void OnKeyPressed(KeyCode code)
		{
			if (code == KeyCode.None)
				return;

			typedStringDisplay.DoKeyPressed(code);
		}

		private void OnGUI()
		{
			if (!showDebug)
				return;

			GUILayout.Label("Virtual Keyboard - current text:" + typedStringDisplay.text);
		}
	}

}
