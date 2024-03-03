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

		[SerializeField, FoldoutGroup("Ambience")]
		private EventReference MineAmbience;

		[SerializeField, FoldoutGroup("Music")]
		private EventReference MineMusic;

		private EventInstance _ambience;
		private EventInstance _music;

		private EventInstance _boss;
		private bool _isPlayingBoss;
		private Func<bool> _onBossFinish;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			RoomManager.Instance.OnRegionChange += OnRegionChange;
			OnRegionChange(RoomManager.Instance.CurrentRegion);
		}

		private void Update()
		{
			bool isInner = RoomManager.Instance.CurrentRoom.IsInnerSection;
			RuntimeManager.StudioSystem.setParameterByName("in-inner-region", isInner ? 1f : 0f);

			if (_isPlayingBoss)
			{
				if (_onBossFinish())
				{
					_isPlayingBoss = false;

					_boss.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					_music.start();
				}
			}
		}

		public static void Play(EventReference sound)
		{
			if (!sound.IsNull)
			{
				RuntimeManager.PlayOneShot(sound);
			}	
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

		public void PlayBossMusic(EventReference sound, Func<bool> onFinish)
		{
			if (!_isPlayingBoss)
			{
				_isPlayingBoss = true;
				_onBossFinish = onFinish;

				_music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

				_boss = RuntimeManager.CreateInstance(sound);
				_boss.start();
			}
		}

		private void OnRegionChange(RegionType type)
		{
			EventReference ambience = type switch
			{
				RegionType.None => throw new Exception("Cannot set region type to 'None'!"),
				RegionType.Mine => MineAmbience,
				_ => throw new NotImplementedException($"Ambience is not implemented for region: '{type}'!")
			};

			_ambience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			_ambience = RuntimeManager.CreateInstance(ambience);
			_ambience.start();

			EventReference music = type switch
			{
				RegionType.Mine => MineMusic,
				_ => throw new NotImplementedException()
			};

			_music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			_music = RuntimeManager.CreateInstance(music);
			_music.start();
		}
	}
}
