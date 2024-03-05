using DG.Tweening;
using Quinn.DamageSystem;
using Quinn.Player;
using Quinn.SpellSystem;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public event Action OnHalfHealth;
		public event Action<State> OnFSMUpdate;

		private Health _health;
		private Damage _damage;
		private bool _isHalfHealth;

		private FSM _fsm;

		private readonly HashSet<Meter> _meters = new();
		private readonly Dictionary<State, UtilityAction> _actions = new();

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

				if (HealthPercent <= 0.5f && !_isHalfHealth)
				{
					_isHalfHealth = true;
					OnHalfHealth?.Invoke();
				}
			};

			_health.OnHeal += OnHealed;
			_health.OnDeath += OnDeath;
		}

		protected virtual void Start()
		{
			_fsm = new FSM(this)
			{
				DebugMode = DebugMode
			};

			OnRegister();
			_fsm.Start();

			_fsm.OnTransition += state =>
			{
				foreach (var delta in _actions[state].Deltas)
				{
					if (_meters.TryGetValue(delta.meter, out var meter))
					{
						meter += delta.delta;
					}
				}
			};

			_fsm.OnUpdate += OnFSMUpdate;
		}

		protected virtual void Update()
		{
			_fsm.Update();

			float bestValue = float.PositiveInfinity;
			State bestState = null;

			foreach (var action in _actions.Values)
			{
				float value = action.CalculateValue();

				if (value < bestValue)
				{
					bestValue = value;
					bestState = action.State;
				}
			}

			if (bestState != null)
			{
				_fsm.TransitionTo(bestState);
			}
		}

		protected virtual void OnDestroy()
		{
			transform.DOKill();
		}

		protected virtual void OnRegister() { }

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

		protected void SetStart(State start)
		{
			_fsm.SetStart(start);
		}

		protected void Connect(State from, State to, Condition condition)
		{
			_fsm.Connect(from, to, condition);
		}

		protected void RegisterState(params State[] states)
		{
			foreach (var state in states)
			{
				_fsm.Register(state);
			}
		}

		protected void RegisterMeter(Meter meter)
		{
			_meters.Add(meter);
		}

		protected void RegisterAction(State state, params (Meter meter, float delta)[] deltas)
		{
			if (!_actions.TryGetValue(state, out UtilityAction action))
			{
				action = new UtilityAction()
				{
					State = state
				};

				_actions.Add(state, action);
			}

			action.Deltas.AddRange(deltas);
		}
	}
}
