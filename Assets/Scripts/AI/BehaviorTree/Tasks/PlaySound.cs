using FMODUnity;
using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class PlaySound : Task
	{
		public EventReference Sound { get; set; }
		public Transform Origin;

		public PlaySound(EventReference sound, Transform origin = null)
		{
			Sound = sound;
			Origin = origin;
		}

		protected override void OnEnter()
		{
			if (Origin != null)
			{
				AudioManager.Play(Sound, Origin.position);
			}
			else
			{
				AudioManager.Play(Sound);
			}
		}
	}
}
