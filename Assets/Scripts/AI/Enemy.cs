using DG.Tweening;
using Quinn.DamageSystem;
using Quinn.Player;
using Quinn.RoomSystem;
using Quinn.SpellSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Health))]
	public abstract class Enemy : MonoBehaviour
	{
		[SerializeField, BoxGroup("Debug")]
		private bool DebugMode;

		public bool IsJumping { get; private set; }

		public Animator Animator { get; private set; }
		public Collider2D Collider { get; private set; }
		public Movement Movement { get; private set; }
		public SpellCaster Caster { get; private set; }
		public bool DisableDamage
		{
			get => _damage.DisableDamage;
			set => _damage.DisableDamage = value;
		}
		public bool UpdateTree { get; set; } = true;

		public float HealthPercent => _health.Percent;
		public float Health => _health.Current;
		public float MaxHealth => _health.Max;
		public Vector2 Position => transform.position;
		public Vector2 Center => Collider.bounds.center;

		public PlayerController Player => PlayerController.Instance;

		public Vector2 TargetPos => Player.transform.position;
		public Vector2 TargetCenter => Player.Center;
		public Vector2 TargetVel => Player.Velocity;
		public float TargetDst => Vector2.Distance(transform.position, TargetPos);
		public Vector2 TargetDir => (TargetPos - Position).normalized;
		public bool DidTargetJustCast => Player.Caster.DidJustCastSpell;

		public bool IsHalfHealth { get; private set; }
		public bool IsDead { get; private set; }

		public event Action OnHalfHealth;

		private Health _health;
		private Damage _damage;

		private Tree _tree;

		protected virtual void Awake()
		{
			Animator = GetComponent<Animator>();
			Collider = GetComponent<Collider2D>();
			_health = GetComponent<Health>();
			Movement = GetComponent<Movement>();
			Caster = GetComponent<SpellCaster>();
			_damage = GetComponent<Damage>();

			GetComponent<Damage>().OnDamaged += (DamageInfo info, DamageEfficiencyType type) =>
			{
				OnDamaged(info, type);

				if (HealthPercent <= 0.5f && !IsHalfHealth)
				{
					IsHalfHealth = true;
					OnHalfHealth?.Invoke();
				}
			};

			_health.OnHeal += OnHealed;
			_health.OnDeath += () =>
			{
				IsDead = true;
				OnDeath();
			};
		}

		protected virtual void Start()
		{
			_tree = ConstructTree();
			_tree.SetAgent(this);
			_tree.DebugMode = DebugMode;
		}

		protected virtual void Update()
		{
			if (UpdateTree)
			{
				_tree.Update();
			}
		}

		protected virtual void OnDestroy()
		{
			transform.DOKill();
		}

		protected virtual Tree ConstructTree() => new();

		protected virtual void OnHealed(float health) { }

		protected virtual void OnDamaged(DamageInfo info, DamageEfficiencyType type) { }

		protected virtual void OnDeath()
		{
			Destroy(gameObject);
		}

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

		protected bool HasLoS(Vector2 target)
		{
			var hit = Physics2D.Linecast(Collider.bounds.center, target, GameManager.Instance.ObstacleLayer);
			return !hit;
		}

		protected T GetChild<T>(string name = "") where T : Node
		{
			return _tree.GetChild<T>(name);
		}
	}
}
