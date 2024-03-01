using FMOD.Studio;
using FMODUnity;
using Quinn.RoomSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }

		[SerializeField, BoxGroup("Ambience")]
		private EventReference MineAmbience;

		private EventInstance _ambience;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			RoomManager.Instance.OnRegionChange += OnRegionChange;
			OnRegionChange(RoomManager.Instance.CurrentRegion);
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

		private void OnRegionChange(RegionType type)
		{
			EventReference sound = type switch
			{
				RegionType.None => throw new Exception("Cannot set region type to 'None'!"),
				RegionType.Mine => MineAmbience,
				_ => throw new NotImplementedException($"Ambience is not implemented for region: '{type}'!")
			};

			_ambience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			_ambience = RuntimeManager.CreateInstance(sound);
			_ambience.start();
		}
	}
}
