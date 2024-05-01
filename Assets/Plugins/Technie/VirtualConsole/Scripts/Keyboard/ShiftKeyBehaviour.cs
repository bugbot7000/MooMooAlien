using UnityEngine;

namespace Technie.VirtualConsole
{
	public class ShiftKeyBehaviour : MonoBehaviour
	{
		private enum State
		{
			Normal,
			SoftLock,
			HardLock
		}

		public bool IsShiftActive { get { return state == State.SoftLock || state == State.HardLock; } }

		// Inspector Properties

		public GameObject keyboardRoot;
		public Key shiftKeyController;
		public Renderer keyRenderer;

		public GameObject Housing;
		public GameObject keyCap;

		// Internal State

		private State state = State.Normal;
		private State nextState = State.Normal;

		private Key[] keyControllers;

		void Start()
		{
			keyControllers = keyboardRoot.GetComponentsInChildren<Key>(true);
		}

		void OnEnable()
		{
			Key.keyPressed += ShiftKeyPressed;
		}

		void OnDisable()
		{
			Key.keyPressed -= ShiftKeyPressed;
		}

		void ShiftKeyPressed(Key srcKey)
		{
			if (srcKey == shiftKeyController)
			{
				if (state == State.Normal)
				{
					nextState = State.SoftLock;
				}
				else if (state == State.SoftLock)
				{
					nextState = State.HardLock;
				}
				else if (state == State.HardLock)
				{
					nextState = State.Normal;
				}
			}
			else
			{
				// If we pressed anything else then remove soft lock
				if (state == State.SoftLock)
					nextState = State.Normal;
			}
		}

		void LateUpdate()
		{
			// Defer state change until the end of the frame so we don't change our state while other keys are processing logic
			if (nextState != state)
			{
				state = nextState;
				ApplyNewState();
			}
		}

		private void ApplyNewState()
		{
			bool nowUpper = (state == State.SoftLock || state == State.HardLock);

			for (int i = 0; i < keyControllers.Length; i++)
			{
				keyControllers[i].SwitchKeycapCharCase(nowUpper);
			}

			Color newCol = (state == State.Normal || state == State.SoftLock) ? shiftKeyController.InitialKeycapColor : shiftKeyController.PressedKeycapColor;
			shiftKeyController.KeycapColor = newCol;
		}

		public void ShiftVisibilityToggle(bool state)
		{
			keyRenderer.enabled = state;
			keyCap.SetActive(state);
			Housing.SetActive(state);

			shiftKeyController.KeycapColor = shiftKeyController.InitialKeycapColor;
		}


	}

}