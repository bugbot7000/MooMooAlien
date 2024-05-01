using UnityEngine;
using UnityEngine.UI;

namespace Technie.VirtualConsole
{
	public class Key : MonoBehaviour
	{
		public enum EntryType
		{
			TypeCharacter,
			TypeKeyCode,
			ClickButton
		}

		public enum CollisionType
		{
			Cylinder,
			Box
		}

		private const float DISTANCE_TO_BE_PRESSED = 0.01f;

		public delegate void OnKeyPressed(Key srcKey);
		public static OnKeyPressed keyPressed;

		// Inspector Properties

		[Header("Collision")]
		public CollisionType collisionType = CollisionType.Cylinder;

		public float cylinderRadius = 0.029f;
		public float cylinderDepth = 0.0165f;

		public Vector3 boxSize = new Vector3(0.1f, 0.2f, 0.3f);

		[Header("Key")]
		public EntryType type = EntryType.TypeCharacter;

		public string KeyCapChar;
		public string AlterateKeyCapChar;

		public KeyCode keyCode;
		public KeyCode altKeyCode;

		public Button targetButton;

		[Header("Properties")]
		public bool KeyPressed = false;
		public Color PressedKeycapColor;
		public Color KeycapColor;
		public Color InitialKeycapColor;

		public KeySoundController keySoundController;

		// Internal Data

		private Transform initialPosition;
		private Text keyCapText;
		private bool symbolSwitch = false;
		private bool checkForButton = true;
		private float currentDistance = -1;

		void Start()
		{
			keyCapText = this.gameObject.GetComponentInChildren<Text>();
			KeycapColor = this.gameObject.GetComponent<Renderer>().material.color;
			InitialKeycapColor = KeycapColor;

			initialPosition = new GameObject(string.Format("[{0}] initialPosition", this.gameObject.name)).transform;
			initialPosition.parent = this.transform.parent;
			initialPosition.localPosition = Vector3.zero;
			initialPosition.localRotation = Quaternion.identity;

			SwitchKeycapCharCase(false);
		}

		void FixedUpdate()
		{
			currentDistance = Vector3.Distance(this.transform.position, initialPosition.position);
		}

		void Update()
		{
			if (checkForButton)
			{
				if (currentDistance > DISTANCE_TO_BE_PRESSED)
				{
					VirtualKeyboard vk = GetComponentInParent<VirtualKeyboard>();

					KeyPressed = true;
					keyPressed(this);
					if (symbolSwitch)
					{
						if (type == EntryType.TypeCharacter)
						{
							vk.OnKeyPressed(AlterateKeyCapChar);
						}
						else if (type == EntryType.TypeKeyCode)
						{
							vk.OnKeyPressed(altKeyCode);
						}
						else if (type == EntryType.ClickButton)
						{

						}
					}
					else
					{
						if (type == EntryType.TypeCharacter)
						{
							vk.OnKeyPressed(KeyCapChar);
						}
						else if (type == EntryType.TypeKeyCode)
						{
							vk.OnKeyPressed(keyCode);
						}
					}

					if (keySoundController != null)
						keySoundController.StartKeySound(this.gameObject.transform);

					checkForButton = false;
				}
			}
			else if (!checkForButton)
			{
				if (currentDistance < DISTANCE_TO_BE_PRESSED)
				{
					KeyPressed = false;
					checkForButton = true;
				}
			}

			ChangeKeyColorOnPress();
		}

		void ChangeKeyColorOnPress()
		{
			if (KeyPressed)
			{
				gameObject.GetComponent<Renderer>().material.color = PressedKeycapColor;
			}
			else
			{
				gameObject.GetComponent<Renderer>().material.color = KeycapColor;
			}
		}

		public void SwitchKeycapCharCase(bool nowUpper)
		{
			if (type == EntryType.TypeCharacter)
			{
				if (nowUpper)
				{
					keyCapText.text = KeyCapChar.ToUpper();
				}
				else
				{
					keyCapText.text = KeyCapChar.ToLower();
				}
			}
		}

		public void SwitchToSymbols()
		{
			if (!symbolSwitch)
			{
				keyCapText.text = AlterateKeyCapChar;
				symbolSwitch = true;
			}
			else
			{
				keyCapText.text = KeyCapChar;
				keyCapText.text = KeyCapChar.ToLower();
				symbolSwitch = false;
			}
		}
	}

} // namespace Technie.VirtualConsole
