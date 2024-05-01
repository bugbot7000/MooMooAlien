
using UnityEngine;

namespace Technie.VirtualConsole
{
	/*
	 *	Unified logging point for all of Technie Virtual Console
	 *	Turn on/off logging via IS_DEBUG_OUTPUT_ENABLED
	 * 
	 *  Example usage:
	 *		Console.output.Log("Hello world");
	 *		Console.output.LogWarning(Console.Technie, "Something not right");
	 */
	public static class Console
	{
		// Turn on/off console logging
		public const bool IS_DEBUG_OUTPUT_ENABLED = false;

		// Tag for LogWarning / LogError
		public static string Technie = "Technie.VirtualConsole";

#if UNITY_2018_1_OR_NEWER
		public static Logger output = new Logger(Debug.unityLogger.logHandler);
#else
		public static Logger output = new Logger(Debug.unityLogger); // 5.6 syntax
#endif
		
		static Console()
		{
			output.logEnabled = IS_DEBUG_OUTPUT_ENABLED;
		}

	}
}

