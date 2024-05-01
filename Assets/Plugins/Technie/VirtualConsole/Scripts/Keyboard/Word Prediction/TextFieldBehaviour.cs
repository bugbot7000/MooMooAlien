using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

namespace Technie.VirtualConsole
{

	public class TextFieldBehaviour : MonoBehaviour, ISelectHandler
	{
		// Inspector Properties

		public NGramGenerator NGramHandler;

		public InputField inputField;

		// Internal State
		// ..
		
		public void OnSelect(BaseEventData eventData)
		{
			StartCoroutine(DisableHighlight());
		}

		public void MoveCaretToEnd()
		{
			StartCoroutine(DisableHighlight());
		}

		IEnumerator DisableHighlight()
		{
			Color originalTextColor = inputField.selectionColor;
			originalTextColor.a = 0f;

			inputField.selectionColor = originalTextColor;

			// Wait for one frame
			yield return null;

			// Scroll the view with the last character
			inputField.MoveTextEnd(true);

			// Change the caret pos to the end of the text
			inputField.caretPosition = inputField.text.Length;

			originalTextColor.a = 1f;
			inputField.selectionColor = originalTextColor;
		}
	}
}