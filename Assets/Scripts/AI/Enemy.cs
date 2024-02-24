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

		public PlayerController Player => PlayerController.Instance;
		public Vector2 PlayerPosition => Player.transform.position;
		public bool HasSpottedPlayer { get; set; }

		public bool IsJumping { get; private set; }

		private Collider2D _collider;

		private void Awake()
		{
			_collider = GetComponent<Collider2D>();
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
