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

		private readonly List<(Condition condition, State next)> _globalConnections = new();
		private readonly Dictionary<State, HashSet<(Condition condition, State next)>> _states = new();
		private State _previousState;

		private Health _health;

		protected virtual void Awake()
		{
			Animator = GetComponent<Animator>();
			Collider = GetComponent<Collider2D>();
			_health = GetComponent<Health>();
			Movement = GetComponent<Movement>();
			Caster = GetComponent<SpellCaster>();

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
			bool isStart = false;
			if (CurrentState != _previousState)
			{
				isStart = true;
				_previousState = CurrentState;
			}

			if (!BlockGlobalConnections)
			{
				foreach (var (connection, next) in _globalConnections)
				{
					bool success = connection(false);
					if (success)
					{
						TransitionTo(next);
						return;
					}
				}
			}

			if (CurrentState != null)
			{
				bool isExiting = CurrentState(isStart);

				foreach (var (condition, next) in _states[CurrentState])
				{
					bool success = condition(isExiting);
					if (success)
					{
						TransitionTo(next);
						return;
					}
				}
			}
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

		protected void Connect(State to, Condition condition)
		{
			_globalConnections.Add((condition, to));
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
			var hit = Physics2D.Linecast(Collider.bounds.center, target, LayerMask.GetMask("Obstacle"));
			return !hit;
		}

		private void TransitionTo(State state)
		{
			_previousState = CurrentState;
			CurrentState = state;
		}
	}
}
