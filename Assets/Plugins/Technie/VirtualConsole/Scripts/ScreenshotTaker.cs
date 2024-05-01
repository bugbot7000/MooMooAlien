using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technie.VirtualConsole
{
	public class ScreenshotTaker : MonoBehaviour
	{
		public const string SCREENSHOT_OUTPUT_FOLDER = "{{PROJECT_FOLDER}}/Screenshots";

		public HandAbstraction handAbstraction;
		public PanelManager panelManager;

		private string lastScreenshotPath = "";

#if UNITY_2018_2_OR_NEWER
		private ScreenCapture.StereoScreenCaptureMode captureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye;

		public void SetCaptureMode(ScreenCapture.StereoScreenCaptureMode newMode)
		{
			this.captureMode = newMode;
		}
#endif

		public string GetLastSavedScreenshotPath()
		{
			return lastScreenshotPath;
		}

		public Coroutine TakeScreenshot(bool hidePanelsOnScreenshot, int supersamplingAmount)
		{
			return StartCoroutine(TakeScreenshotRoutine(hidePanelsOnScreenshot, supersamplingAmount));
		}

		private IEnumerator TakeScreenshotRoutine(bool hidePanelsOnScreenshot, int supersamplingAmount)
		{
			// Hide hands/panels
			if (hidePanelsOnScreenshot)
			{
				handAbstraction.SetStylusVisible(false);
				panelManager.SetDisplaysVisible(false);
			}
			yield return null;

			// Trigger screenshot

			// Build the output file path and create any directories needed

			string projectDir;
#if UNITY_EDITOR
			projectDir = Application.dataPath.Replace("/Assets", "");
#else
			projectDir = Application.persistentDataPath;
#endif
			string path = SCREENSHOT_OUTPUT_FOLDER.Replace("{{PROJECT_FOLDER}}", projectDir);
			System.IO.Directory.CreateDirectory(path);

			string fileName = "/" + System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".png";
			path += fileName;

#if UNITY_2018_2_OR_NEWER
			ScreenCapture.CaptureScreenshot(path, captureMode);
#else
			ScreenCapture.CaptureScreenshot(path, supersamplingAmount); // NB: Supersampling parameter doesn't work for VR captures
#endif
			this.lastScreenshotPath = path;

			yield return null;

			// Show hands/panels again
			if (hidePanelsOnScreenshot)
			{
				handAbstraction.SetStylusVisible(true);
				panelManager.SetDisplaysVisible(true);
			}
			yield return null;
		}
	}

} // Technie.VirtualConsole