using UnityEngine;

namespace Technie.VirtualConsole
{
	public class KeySoundController : MonoBehaviour
	{
		public AudioSource keySfx;

		public void StartKeySound(Transform keyTransform)
		{
			if (keySfx.isActiveAndEnabled)
				keySfx.Play();
		}
	}
}