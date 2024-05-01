using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.VirtualConsole
{
	public class SymbolsKeyBehaviour : MonoBehaviour
	{
		public GameObject keyboardRoot;
		public ShiftKeyBehaviour ShiftBehaviour;

		private Key symbolKeyController;
		private Key[] keyControllers;
		private bool symbolToggle = true;

		void Start()
		{
			Key.keyPressed += SpecialKeyPressed;

			symbolKeyController = this.gameObject.GetComponent<Key>();
			keyControllers = keyboardRoot.GetComponentsInChildren<Key>(true);
		}

		void SpecialKeyPressed(Key srcKey)
		{
			if (srcKey == symbolKeyController)
			{
				for (int i = 0; i < keyControllers.Length; i++)
				{
					keyControllers[i].SwitchToSymbols();
				}

				if (symbolToggle)
				{
					ShiftBehaviour.ShiftVisibilityToggle(false);
					symbolKeyController.KeycapColor = symbolKeyController.PressedKeycapColor;
					symbolToggle = false;
				}
				else if (!symbolToggle)
				{
					ShiftBehaviour.ShiftVisibilityToggle(true);
					symbolKeyController.KeycapColor = symbolKeyController.InitialKeycapColor;
					symbolToggle = true;
				}
			}
		}

		void OnDisable()
		{
			Key.keyPressed -= SpecialKeyPressed;
		}
	}
}
