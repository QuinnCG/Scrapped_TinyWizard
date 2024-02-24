using DG.Tweening;
using Quinn.Player;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Health))]
	public abstract class Enemy : MonoBehaviour
	{
		[SerializeField]
		private bool DisplayBossBar;

		[field: SerializeField, ShowIf(nameof(DisplayBossBar)), BoxGroup("Core")]
		public string BossBarTitle { get; private set; } = "Enemy Title";

		[SerializeField, BoxGroup("Core")]
		private float PlayerSpotRadius = 8f;

		public bool IsJumping { get; private set; }

		protected PlayerController Player => PlayerController.Instance;
		protected Vector2 PlayerPos => Player.transform.position;
		protected Vector2 PlayerCenter => Player.Center;
		protected Vector2 PlayerVel => Player.Velocity;

		protected float HealthPercent => _health.Percent;
		protected float Health => _health.Current;
		protected float MaxHealth => _health.Max;

		private Collider2D _collider;
		private Health _health;

		private void Awake()
		{
			_collider = GetComponent<Collider2D>();
			_health = GetComponent<Health>();
		}

		protected void JumpTo(Vector2 position, float height, float speed)
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

		protected bool HasLineOfSight(Vector2 target)
		{
			var hit = Physics2D.Linecast(_collider.bounds.center, target, LayerMask.GetMask("Obstacle"));
			return hit.collider == null;
		}
	}
}
