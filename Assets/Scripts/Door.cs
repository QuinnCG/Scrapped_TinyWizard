using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
	public class Door : MonoBehaviour
	{
		[SerializeField, BoxGroup("Default")]
		private bool StartOpen = true;

		[SerializeField, BoxGroup("Default")]
		private bool StartLocked = false;

		[SerializeField, BoxGroup("Sprites"), Required]
		private Sprite OpenSprite, CloseSprite;

		[SerializeField, BoxGroup("Sound")]
		private EventReference OpenSound, CloseSound;

		[SerializeField, BoxGroup("Sound"), ShowIf(nameof(StartLocked))]
		private EventReference UnlockSound;

		public bool IsOpen { get; private set; }
		public bool IsLocked { get; private set; }

		private SpriteRenderer _renderer;
		private Collider2D _collider;

		private void Awake()
		{
			_renderer = GetComponent<SpriteRenderer>();
			_collider = GetComponent<Collider2D>();

			if (StartOpen)
			{
				Open(true);
			}
			else
			{
				Close(true);
			}

			IsLocked = StartLocked;
		}

		public void Unlock()
		{
			if (IsLocked)
			{
				AudioManager.Play(UnlockSound, transform.position);
			}
		}

		public void Open(bool supressFX)
		{
			if (!IsOpen)
			{
				_collider.enabled = false;
				_renderer.sprite = OpenSprite;

				if (!supressFX)
				{
					AudioManager.Play(OpenSound, transform.position);
				}
			}
		}

		public void Close(bool supressFX)
		{
			if (IsOpen)
			{
				_collider.enabled = true;
				_renderer.sprite = CloseSprite;

				if (!supressFX)
				{
					AudioManager.Play(CloseSound, transform.position);
				}
			}
		}
	}
}
