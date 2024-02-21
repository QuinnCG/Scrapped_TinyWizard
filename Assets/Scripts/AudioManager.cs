using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
		}

		public static void Play(EventReference sound, Vector2 position)
		{
			if (!sound.IsNull)
			{
				RuntimeManager.PlayOneShot(sound, position);
			}
		}
		public static void Play(EventReference sound, Transform parent)
		{
			if (!sound.IsNull)
			{
				RuntimeManager.PlayOneShotAttached(sound, parent.gameObject);
			}
		}
	}
}
