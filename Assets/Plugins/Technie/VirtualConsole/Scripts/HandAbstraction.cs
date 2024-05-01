using UnityEngine;
using UnityEngine.EventSystems;

namespace Technie.VirtualConsole
{
	public class HandAbstraction : MonoBehaviour
	{
		public VirtualConsole virtualConsole;

		public Material ballMaterial;
		public Material laserMaterial;

		public Sprite cursorSprite;
		public Material cursorMaterial;

		public XrNodeRig xrRig;
		public PanelManager panelManager;
		
		// Internal State

		private WandInputModule wandInputModule;

		private BaseInputModule[] allInputModules = new BaseInputModule[0];
		
		private UiStylus leftStylus;
		private UiStylus rightStylus;

		void OnEnable()
		{

		}
		
		void OnDisable()
		{

		}

		void Start()
		{
			// Always call this on start so we refetch hands if we're dynamically loaded as a prefab after SteamVR has sent all of it's controller events

			FindHands();
		}

		private void FindHands()
		{
			// Lazily create the wand input module

			if (wandInputModule == null)
			{
#if UNITY_2020_1_OR_NEWER
				// 2020 compatible input handling means reusing an existing event system and adding our processing onto it
				// Since we can only have one input module active at once, we dynamically toggle them on and off based on the
				// proximity of the styluses

				EventSystem sys = Object.FindObjectOfType<EventSystem>();
				if (sys == null)
				{
					// No existing event system to piggy back on, so we'll create one ourselves
					GameObject sysObj = new GameObject("Event System");
					sysObj.transform.SetParent(this.transform, false);
					sys = sysObj.AddComponent<EventSystem>();
				}
				
				wandInputModule = sys.gameObject.AddComponent<WandInputModule>();
				wandInputModule.CursorSprite = cursorSprite;
				wandInputModule.CursorMaterial = cursorMaterial;
				wandInputModule.enabled = false;

				allInputModules = sys.GetComponents<BaseInputModule>();
#else
				// Isolated event system used in 2017/2018/2019
				// Note this does work on 2020, but it triggers a "Multiple event systems" warning every frame which floods the console

				GameObject eventSystemObj = new GameObject("Isolated Event System");
				eventSystemObj.transform.SetParent(this.transform, false);

				eventSystemObj.gameObject.AddComponent<IsolatedEventSystem>();

				wandInputModule = eventSystemObj.gameObject.AddComponent<WandInputModule>();
				wandInputModule.CursorSprite = cursorSprite;
				wandInputModule.CursorMaterial = cursorMaterial;
#endif
			}

			// Lazily create the styluses

			if (leftStylus == null)
			{
				leftStylus = CreateStylus(HandType.Left, xrRig.leftHandTransform, wandInputModule);
				
				SetStylusPosition(leftStylus, HandType.Left, virtualConsole.leftStylusPosition, virtualConsole.customLeftStylusPosition);
			}

			if (rightStylus == null)
			{
				rightStylus = CreateStylus(HandType.Right, xrRig.rightHandTransform, wandInputModule);

				SetStylusPosition(rightStylus, HandType.Right, virtualConsole.rightStylusPosition, virtualConsole.customRightStylusPosition);
			}

			wandInputModule = GameObject.FindObjectOfType(typeof(WandInputModule)) as WandInputModule;
			if (wandInputModule != null)
			{
				wandInputModule.SetRaycastOrigins(leftStylus.transform, rightStylus.transform);

				wandInputModule.OnHandsDetected(this); // Needed to initialise input module

				panelManager.OnHandsDetected(wandInputModule.GetControllerCamera());
			}
		}

		private UiLaser CreateLaser()
		{
			GameObject laser = new GameObject ("Ui Laser");
			laser.transform.SetParent (this.transform, false);

			UiLaser laserComponent = laser.AddComponent<UiLaser> ();
			laserComponent.CreateBeam (laserMaterial);

			return laserComponent;
		}

		private UiStylus CreateStylus(HandType type, Transform handTransform, WandInputModule inputModule)
		{
			GameObject obj = new GameObject("Ui Stylus");
			obj.transform.SetParent(handTransform, false);

			UiStylus stylus = obj.AddComponent<UiStylus>();
			stylus.laserMaterial = laserMaterial;
			stylus.ballMaterial = ballMaterial;
			stylus.handType = type;
			stylus.inputModule = inputModule;

			return stylus;
		}

		private void SetStylusPosition(UiStylus stylus, HandType hand, StylusPosition stylusPosition, Vector3 customStylusPosition)
		{
			switch (stylusPosition)
			{
				case StylusPosition.Top:
				{
					stylus.transform.localPosition = new Vector3(0.0f, 0.0f, 0.05f);
					break;
				}
				case StylusPosition.Bottom:
				{
					stylus.transform.localPosition = new Vector3(0.0f, -0.01f, -0.18f);
					break;
				}
				case StylusPosition.Left:
				{
					stylus.transform.localPosition = new Vector3(-0.085f, -0.01f, -0.01f);
					break;
				}
				case StylusPosition.Right:
				{
					stylus.transform.localPosition = new Vector3( 0.085f, -0.01f, -0.01f);
					break;
				}
				case StylusPosition.Custom:
				{
					stylus.transform.localPosition = customStylusPosition;
					break;
				}
			}
		}
		
		public GameObject GetLeftHand()
		{
			return null;
		}

		public GameObject GetRightHand()
		{
			return null;
		}

		public bool HasTarget(HandType targetHand)
		{
			if (targetHand == HandType.Left)
			{
				return leftStylus != null ? leftStylus.IsPointingAtPanel() : false;
			}
			else if (targetHand == HandType.Right)
			{
				return rightStylus != null ? rightStylus.IsPointingAtPanel() : false;
			}
			return false;
		}

		public void TriggerInput(HandType targetHand)
		{
			// No longer used
		}

		private void Update()
		{
#if UNITY_2020_1_OR_NEWER
			bool stylusNearCanvas = leftStylus.IsPointingAtPanel() || rightStylus.IsPointingAtPanel();

			// only enable/disable input modules if status has changed, as otherwise they get stuck in one-frame initialization and don't react
			if (stylusNearCanvas != wandInputModule.enabled)
			{
				EnableWandInputModule(stylusNearCanvas);
			}
#endif
		}

		private void EnableWandInputModule(bool wandEnable)
		{
			// Enable the wand input module, and disable all other modules (or vice versa)

			foreach (BaseInputModule mod in allInputModules)
			{
				if (mod != wandInputModule)
					mod.enabled = !wandEnable;
			}
			wandInputModule.enabled = wandEnable;
		}

		public void SetStylusVisible(bool visible)
		{
			if (leftStylus != null)
				leftStylus.gameObject.SetActive(visible);
			if (rightStylus != null)
				rightStylus.gameObject.SetActive(visible);

			if (wandInputModule != null)
				wandInputModule.SetCursorsVisible(visible);
		}
	}
}
