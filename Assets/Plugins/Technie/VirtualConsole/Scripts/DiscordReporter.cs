using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Technie.VirtualConsole
{
	public class DiscordReporter : MonoBehaviour
	{
		public struct ReportData
		{
			public string content; // the message content, max 2000 characters
			public EmbedData[] embeds;
		}

		[System.Serializable]
		public struct EmbedData
		{
			public string title;
			public string description;
			public string url;
			public int color;

			public EmbedImage image;
			public EmbedThumbnail thumbnail;
			public EmbedAuthor author;
			public EmbedFooter footer;

			public EmbedField[] fields;
		}

		[System.Serializable]
		public struct EmbedImage
		{
			public string url;
			public int height;
			public int width;
		}

		[System.Serializable]
		public struct EmbedField
		{
			public string name;
			public string value;
			public bool inline;
		}

		[System.Serializable]
		public struct EmbedThumbnail
		{
			public string url;
			public int height;
			public int width;
		}


		[System.Serializable]
		public struct EmbedFooter
		{
			public string text;
			public string icon_url;
		}

		[System.Serializable]
		public struct EmbedAuthor
		{
			public string name;
			public string url;
			public string icon_url;
		}

		public string ErrorMessage
		{
			get { return this.errorMessage; }
		}

		// Inspector Properties

		public XrNodeRig xrNodeRig;

		public VrConsole consolePanel;
		public VrDebugStats statsPanel;

		public Texture2D thumbnail;
		public Texture2D footerIcon;

		public VirtualConsole console;

		// Internal State
		
		private string errorMessage = "";

		public Coroutine PostReport(string bugName, string bugDescription, string screenshotPath)
		{
			return StartCoroutine(PostReportRoutine(bugName, bugDescription, screenshotPath));
		}

		private IEnumerator PostReportRoutine(string bugName, string bugDescription, string screenshotPath)
		{
			Console.output.Log("Assembling bug report");

			errorMessage = "";

			if (string.IsNullOrEmpty(console.discordBugReportingWebhook))
			{
				errorMessage = string.Format("no Discord webhook set on Virtual Console prefab - see documentation");
				yield break;
			}

			// Gather info to send with the bug report

			string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
#if UNITY_2020_1_OR_NEWER
			string vrDevice = UnityEngine.XR.XRSettings.loadedDeviceName;
#else
			string vrDevice = UnityEngine.XR.XRDevice.model;
#endif
			string computerName = SystemInfo.deviceName;
			string timeStamp = System.DateTime.Now.ToString("dd/MM/yyyy @ HH:mm:ss");
			string consoleLog = consolePanel.GetAllLogs();
			string statsStr = statsPanel.GetAllStats();

			string cameraLocation = null;
			Camera vrCamera = xrNodeRig.GetVrCamera();
			if (vrCamera == null)
				vrCamera = Camera.main;
			if (vrCamera != null)
			{
				cameraLocation = string.Format("Camera position: {0}\nCamera rotation: {1}", vrCamera.transform.position.ToString(), vrCamera.transform.rotation.eulerAngles.ToString());
			}

			string webhookUrl = console.discordBugReportingWebhook;

			// Build the message POD structure we'll convert into our json payload

			string fullMessage = bugDescription;
			if (cameraLocation != null)
				fullMessage += "\n\n" + cameraLocation;

			EmbedData embed = new EmbedData();
			embed.title = "**" + bugName +"**";
			embed.description = fullMessage;
			embed.url = "";
			embed.color = 0xFF217C;
			embed.footer.text = "Reported on " + timeStamp;
			embed.footer.icon_url = "attachment://icon.png";

			embed.author.name = "Technie Bug Report";
			embed.author.url = "https://assetstore.unity.com/packages/tools/utilities/technie-virtual-console-57548";
			embed.author.icon_url = "";

			embed.thumbnail.url = "attachment://thumbnail.png";
			embed.image.url = "attachment://screenshot.png";

			embed.fields = new EmbedField[]
			{
				new EmbedField()
				{
					name = "Platform",
					value = Application.platform.ToString(),
					inline = true
				},
				new EmbedField()
				{
					name = "VR Device",
					value = vrDevice,
					inline = true
				},
				new EmbedField()
				{
					name = "Reporter",
					value = computerName,
					inline = true
				},
				new EmbedField()
				{
					name = "Scene",
					value = sceneName,
					inline = true
				},	
			};

			ReportData data = new ReportData();
			data.content = "";
			data.embeds = new EmbedData[] { embed };

			// Convert the message POD data to json string

			string jsonStr = JsonUtility.ToJson(data, true);

			// Now assemble the form data (json + attachments)

			WWWForm form = new WWWForm();
			form.AddField("payload_json", jsonStr);
			
			byte[] thumbnailData = thumbnail.EncodeToPNG();
			form.AddBinaryData("file0", thumbnailData, "thumbnail.png", "image/png");
			
			byte[] iconData = footerIcon.EncodeToPNG();
			form.AddBinaryData("file1", iconData, "icon.png", "image/png");

			if (screenshotPath != null && File.Exists(screenshotPath))
			{
				byte[] screenshotData = File.ReadAllBytes(screenshotPath);
				form.AddBinaryData("file2", screenshotData, "screenshot.png", "image/png");
			}

			// Create the request with the form data and dispatch it

			UnityWebRequest request = UnityWebRequest.Post(webhookUrl, form);
			yield return null;

			Console.output.Log("Sending bug report...");
			yield return request.SendWebRequest();

			Console.output.Log("Got result: " + request.responseCode);
			Console.output.Log("Reponse: " + request.downloadHandler.text);

			if (HasError(request))
			{
				errorMessage = "Couldn't send bug report - see console for details";
				Debug.LogError(string.Format("[VirtualConsole] Received error when submitting bug report - code '{0}' description '{1}' response '{2}'", request.responseCode, request.error, request.downloadHandler.text));
				yield break;
			}

			// Now we've send the main bug report message send a separate message with the logs and the stats
			// Ideally we'd do this as one big message but discord doesn't seem to support message+screenshot+thumbnails+logs+stats all at once

			ReportData consoleData = new ReportData();

			WWWForm consoleForm = new WWWForm();
			form.AddField("payload_json", JsonUtility.ToJson(consoleData, true));
			consoleForm.AddBinaryData("log", Encoding.UTF8.GetBytes(consoleLog), bugName+" Log.txt", "application/text");
			consoleForm.AddBinaryData("stats", Encoding.UTF8.GetBytes(statsStr), bugName+" Stats.txt", "application/text");
			UnityWebRequest consoleReq = UnityWebRequest.Post(webhookUrl, consoleForm);
			yield return consoleReq.SendWebRequest();
			
			if (HasError(consoleReq))
			{
				errorMessage = "Couldn't attach logs to bug report - see console for details";
				Debug.LogError(string.Format("[VirtualConsole] Received error when submitting bug logs - code '{0}' description '{1}' response '{2}'", consoleReq.responseCode, consoleReq.error, consoleReq.downloadHandler.text));
				yield break;
			}

			Console.output.Log("Finished sending bug report");
		}

		private static bool HasError(UnityWebRequest request)
		{
#if UNITY_2020_1_OR_NEWER
			bool hasError = (request.result != UnityWebRequest.Result.Success);
#else
			bool hasError = request.isNetworkError || request.isHttpError;
#endif
			return hasError;
		}

		[ContextMenu("Test Discord report")]
		public void TestDiscordReport()
		{
			PostReport("Test Bug Title Here",
					"When I rotated the gromit, sometimes it only emits a bloop, when it should emit several blarps",
					"E:\\Image Reference\\Characters\\GrumpyCat.png");
		}
	}

} // Technie.VirtualConsole
