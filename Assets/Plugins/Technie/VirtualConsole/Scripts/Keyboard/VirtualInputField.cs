using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technie.VirtualConsole
{
	public class VirtualInputField : InputField
	{
		public void DoKeyPressed(KeyCode code)
		{
			Event e = new Event();
			e.keyCode = code;

			base.KeyPressed(e);
			base.ForceLabelUpdate();
		}

		public void DoAppend(char ch)
		{
			base.Append(ch);
			base.ForceLabelUpdate();
		}
	}
}
