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
		protected bool IsJumping { get; private set; }

		protected Animator Animator { get; private set; }
		protected Collider2D Collider { get; private set; }
		protected Movement Movement { get; private set; }
		protected SpellCaster Caster { get; private set; }
		protected bool DisableDamage
		{
			get => _damage.DisableDamage;
			set => _damage.DisableDamage = value;
		}

		protected float HealthPercent => _health.Percent;
		protected float Health => _health.Current;
		protected float MaxHealth => _health.Max;
		protected Vector2 Position => transform.position;

		protected PlayerController Player => PlayerController.Instance;
		protected Vector2 PlayerPos => Player.transform.position;
		protected Vector2 PlayerCenter => Player.Center;
		protected Vector2 PlayerVel => Player.Velocity;
		protected float PlayerDst => Vector2.Distance(transform.position, PlayerPos);
		protected Vector2 PlayerDir => (PlayerPos - Position).normalized;

		protected State CurrentState { get; private set; }
		protected bool BlockGlobalConnections { get; set; }

		private readonly Dictionary<State, HashSet<(Condition condition, State next)>> _states = new();

		private Health _health;
		private Damage _damage;

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
			OnRegisterStates();
		}

		protected virtual void Update()
		{
			/*
			 * IEnumerator-based states that can suspend their execution.
			 * Custom yield instructions.
			 * States are transitioned to via connections.
			 * States are their own classes that have the IEnumerator OnUpdate(), void OnStart(), and void OnFinish(bool isInterruption).
			 */
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

		protected void Register(State state)
		{
			_states.Add(state, new());
		}
		protected void Register(params State[] states)
		{
			foreach (var state in states)
			{
				Register(state);
			}
		}

		protected void Connect(State from, State to, Condition condition)
		{
			if (_states.TryGetValue(from, out var connections))
			{
				connections.Add((condition, to));
			}
		}

		protected void SetStartState(State start)
		{
			CurrentState = start;
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

		private void TransitionTo(State state)
		{
			CurrentState = state;
		}
	}
}
