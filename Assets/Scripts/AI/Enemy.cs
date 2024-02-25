using DG.Tweening;
using Quinn.DamageSystem;
using Quinn.Player;
using Quinn.SpellSystem;
using Quinn.UI;
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
		protected bool IsJumping { get; private set; }

		protected Movement Movement { get; private set; }
		protected SpellCaster Caster { get; private set; }

		protected float HealthPercent => _health.Percent;
		protected float Health => _health.Current;
		protected float MaxHealth => _health.Max;
		protected Vector2 Position => transform.position;

		protected PlayerController Player => PlayerController.Instance;
		protected Vector2 PlayerPos => Player.transform.position;
		protected Vector2 PlayerCenter => Player.Center;
		protected Vector2 PlayerVel => Player.Velocity;
		protected float PlayerDistance => Vector2.Distance(transform.position, PlayerPos);

		private Collider2D _collider;
		private Health _health;

		protected virtual void Awake()
		{
			_collider = GetComponent<Collider2D>();
			_health = GetComponent<Health>();
			Movement = GetComponent<Movement>();
			Caster = GetComponent<SpellCaster>();
		}

		protected virtual void Start() { }

		protected virtual void Update() { }

		protected Tween JumpTo(Vector2 position, float height, float duration)
		{
			if (!IsJumping)
			{
				IsJumping = true;

				var tween = transform.DOJump(position, height, 1, duration)
					.SetEase(Ease.Linear);

				tween.onKill += () => IsJumping = false;
				tween.onComplete += () => IsJumping = false;

				return tween;
			}

			return null;
		}

		protected bool HasLineOfSight(Vector2 target)
		{
			var hit = Physics2D.Linecast(_collider.bounds.center, target, LayerMask.GetMask("Obstacle"));
			return !hit;
		}
	}
}
