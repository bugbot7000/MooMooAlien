using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technie.VirtualConsole
{
	public class DiscordBugReportPanel : DebugPanel
	{
		public GameObject keyboardPrefab;

		public XrNodeRig xrRig;
		public HandAbstraction handAbstraction;
		public PanelManager panelManager;

		public ScreenshotTaker screenshotTaker;
		public DiscordReporter reporter;
		public Text screenshotInfo;

		public InputField bugTitleInput;
		public InputField bugDescriptionInput;

		public Text uploadStatusDisplay;

		// Internal State

		private string screenshotPath;

		private VirtualKeyboard activeKeyboard;

		void Start()
		{
			screenshotInfo.text = "No screenshot";
			screenshotPath = "";

			uploadStatusDisplay.text = "";
		}

		public override void OnAttach() { }
		public override void OnDetach() { }
		public override void OnResized(VrDebugDisplay.State size) { }

		void Update()
		{

		}

		public void OnTakeScreenshot()
		{
			StartCoroutine(TakeScreenshotRoutine());
		}

		private IEnumerator TakeScreenshotRoutine()
		{
			// Do countdown
			
			float timeLeft = 4.0f;
			while (timeLeft > 0f)
			{
				timeLeft -= Time.deltaTime;
				screenshotInfo.text = string.Format("Taking screenshot in {0}...", timeLeft.ToString("0.0"));
				yield return null;
			}

			// Take the screenshot

			yield return screenshotTaker.TakeScreenshot(true, 1);
			screenshotPath = screenshotTaker.GetLastSavedScreenshotPath();

			screenshotInfo.text = "Screenshot ready";

			uploadStatusDisplay.text = "";
		}

		public void OnEditTitle()
		{
			ShowKeyboard(bugTitleInput.text);
			activeKeyboard.OnKeyboardFinished += OnEditTitleFinished;
		}

		public void OnEditDescription()
		{
			ShowKeyboard(bugDescriptionInput.text);
			activeKeyboard.OnKeyboardFinished += OnEditDescriptionFinished;
		}

		private void OnEditTitleFinished(bool isOk, string enteredText)
		{
			// Update the bug with the new text
			bugTitleInput.text = enteredText;

			HideKeyboard();
		}

		private void OnEditDescriptionFinished(bool isOk, string enteredText)
		{
			// Update the bug with the new text
			bugDescriptionInput.text = enteredText;

			HideKeyboard();
		}

		private void ShowKeyboard(string initialText)
		{
			// Find the camera's flat forward vector
			Camera vrCamera = xrRig.GetVrCamera();
			Vector3 flatForward = vrCamera.transform.forward;
			flatForward.y = 0f;
			flatForward.Normalize();

			// Instantiate the keyboard prefab
			GameObject keyboardObj = GameObject.Instantiate(keyboardPrefab);
			keyboardObj.transform.position = vrCamera.transform.position + (flatForward * 0.4f) + new Vector3(0f, -0.6f, 0f);
			keyboardObj.transform.rotation = Quaternion.LookRotation(flatForward);
			keyboardObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

			// Pass on the initial text to start with
			activeKeyboard = keyboardObj.GetComponent<VirtualKeyboard>();
			activeKeyboard.SetText(initialText);

			// Attach the drumsticks to the player's hands
			activeKeyboard.leftDrumstick.transform.SetParent(xrRig.leftHandTransform, false);
			activeKeyboard.leftDrumstick.transform.localPosition = Vector3.zero;
			activeKeyboard.leftDrumstick.transform.localRotation = Quaternion.identity;

			activeKeyboard.rightDrumstick.transform.SetParent(xrRig.rightHandTransform, false);
			activeKeyboard.rightDrumstick.transform.localPosition = Vector3.zero;
			activeKeyboard.rightDrumstick.transform.localRotation = Quaternion.identity;

			// Disable the panels while we're entering text
			this.panelManager.SetDisplaysVisible(false);

			// Hide the panel styluses while we're entering text
			this.handAbstraction.SetStylusVisible(false);
		}

		private void HideKeyboard()
		{
			// Show the virtual console again
			this.panelManager.SetDisplaysVisible(true);
			this.handAbstraction.SetStylusVisible(true);

			// Destroy the drumsticks
			GameObject.Destroy(activeKeyboard.leftDrumstick);
			GameObject.Destroy(activeKeyboard.rightDrumstick);

			// Destroy the keyboard
			GameObject.Destroy(activeKeyboard.gameObject);
			activeKeyboard = null;
		}

		public void OnSubmitBug()
		{
			StartCoroutine(SubmitBugRoutine());
		}

		private IEnumerator SubmitBugRoutine()
		{
			uploadStatusDisplay.text = "Submitting bug...";

			yield return reporter.PostReport(bugTitleInput.text, bugDescriptionInput.text, screenshotPath);

			// Clear the bug info now we've posted the report
			screenshotPath = "";
			screenshotInfo.text = "No screenshot";
			bugTitleInput.text = "";
			bugDescriptionInput.text = "";

			// Show feedback to user based on post result
			if (reporter.ErrorMessage == "")
			{
				uploadStatusDisplay.text = "Bug submitted successfully";
			}
			else
			{
				uploadStatusDisplay.text = reporter.ErrorMessage;
			}
		}
	}

} // Technie.VirtualConsole
