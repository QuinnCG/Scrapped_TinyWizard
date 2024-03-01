using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public class SFXPlayer : MonoBehaviour
	{
		[SerializeField]
		private EventReference Sound;

		[SerializeField]
		private bool IsAttached;

		public void PlaySound()
		{
			if (IsAttached)
			{
				AudioManager.Play(Sound, transform);
			}
			else
			{
				AudioManager.Play(Sound, transform.position);
			}
		}
	}
}
