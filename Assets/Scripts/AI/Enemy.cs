using DG.Tweening;
using Quinn.DamageSystem;
using Quinn.Player;
using Quinn.SpellSystem;
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
		[SerializeField]
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

		public float HealthPercent => _health.Percent;
		public float Health => _health.Current;
		public float MaxHealth => _health.Max;
		public Vector2 Position => transform.position;

		public PlayerController Player => PlayerController.Instance;
		public Vector2 PlayerPos => Player.transform.position;
		public Vector2 PlayerCenter => Player.Center;
		public Vector2 PlayerVel => Player.Velocity;
		public float PlayerDst => Vector2.Distance(transform.position, PlayerPos);
		public Vector2 PlayerDir => (PlayerPos - Position).normalized;

		private Health _health;
		private Damage _damage;

		private FSM _fsm;

		protected virtual void Awake()
		{
			Animator = GetComponent<Animator>();
			Collider = GetComponent<Collider2D>();
			_health = GetComponent<Health>();
			Movement = GetComponent<Movement>();
			Caster = GetComponent<SpellCaster>();
			_damage = GetComponent<Damage>();

			GetComponent<Damage>().OnDamaged += OnDamaged;
			_health.OnHeal += OnHealed;
			_health.OnDeath += OnDeath;
		}

		protected virtual void Start()
		{
			_fsm = new FSM(this)
			{
				DebugMode = DebugMode
			};

			OnRegisterStates();
			_fsm.Start();
		}

		protected virtual void Update()
		{
			_fsm.Update();
		}

		protected virtual void OnDestroy()
		{
			transform.DOKill();
		}

		protected virtual void OnRegisterStates() { }

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

		protected bool HasLineOfSight(Vector2 target)
		{
			var hit = Physics2D.Linecast(Collider.bounds.center, target, GameManager.Instance.ObstacleLayer);
			return !hit;
		}

		protected void SetStart(State start)
		{
			_fsm.SetStart(start);
		}

		protected void Connect(State from, State to, Condition condition)
		{
			_fsm.Connect(from, to, condition);
		}
	}
}
