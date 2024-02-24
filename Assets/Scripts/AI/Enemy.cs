using DG.Tweening;
using Quinn.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Health))]
	public class Enemy : MonoBehaviour
	{
		[SerializeField]
		private bool DisplayBossBar;

		[field: SerializeField, ShowIf(nameof(DisplayBossBar))]
		public string BossBarTitle { get; private set; } = "Enemy Title";

		[SerializeField]
		private float PlayerSpotRadius = 8f;

		[SerializeField]
		private LayerMask CharacterMask;

		public PlayerController Player => PlayerController.Instance;
		public Vector2 PlayerPosition => Player.transform.position;
		public bool HasSpottedPlayer { get; set; }

		public bool IsJumping { get; private set; }

		private Collider2D _collider;

		private void Awake()
		{
			_collider = GetComponent<Collider2D>();
		}

		private void FixedUpdate()
		{
			if (!HasSpottedPlayer)
			{
				var player = PlayerController.Instance;
				var playerPos = player.transform.position;
				float dst = Vector2.Distance(transform.position, playerPos);

				if (dst < PlayerSpotRadius)
				{
					var hits = Physics2D.LinecastAll(_collider.bounds.center, playerPos, CharacterMask.value);

					foreach (var hit in hits)
					{
						if (hit.collider.gameObject == player)
						{
							HasSpottedPlayer = true;
							break;
						}
					}
				}
			}
		}

		public void JumpTo(Vector2 position, float height, float speed)
		{
			if (!IsJumping)
			{
				IsJumping = true;

				var tween = transform.DOJump(position, height, 1, speed)
					.SetSpeedBased()
					.SetEase(Ease.Linear);

				tween.onKill += () => IsJumping = false;
				tween.onComplete += () => IsJumping = false;
			}
		}
	}
}
