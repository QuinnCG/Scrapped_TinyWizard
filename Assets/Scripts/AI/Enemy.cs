using DG.Tweening;
using Quinn.Player;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

		[SerializeField, BoxGroup("Core")]
		private LayerMask CharacterMask;

		public PlayerController Player => PlayerController.Instance;
		public Vector2 PlayerPosition => Player.transform.position;
		public bool HasSpottedPlayer { get; set; }

		public bool IsJumping { get; private set; }

		protected State ActiveState { get; private set; }
		protected State StartState{ get; set; }

		private Collider2D _collider;

		private readonly HashSet<State> _states = new();
		private readonly Dictionary<State, List<(Func<bool> condition, State to)>> _connections = new();

		protected virtual void Awake()
		{
			_collider = GetComponent<Collider2D>();
		}

		protected virtual void Start()
		{
			TransitionTo(StartState);
		}

		protected virtual void Update()
		{
			if (ActiveState != null)
			{
				bool exitStatus = ActiveState.OnUpdate();

				if (exitStatus)
				{
					ActiveState?.OnExit(false);
					ActiveState = null;
				}
				else
				{
					if (_connections.TryGetValue(ActiveState, out var connections))
					{
						foreach (var (condition, to) in connections)
						{
							if (condition())
							{
								TransitionTo(to);
								break;
							}
						}
					}
				}
			}
		}

		protected virtual void FixedUpdate()
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

		protected void RegisterState(State state)
		{
			state.Agent = this;
			_states.Add(state);
		}

		protected void ConnectState(State from, State to, Func<bool> condition)
		{
			if (_connections.TryGetValue(from, out var connections))
			{
				connections.Add((condition, to));
			}
			else
			{
				_connections.Add(from, new() { (condition, to) });
			}
		}

		private void TransitionTo(State state)
		{
			if (ActiveState != null && !ActiveState.IsInterruptable)
			{
				return;
			}

			ActiveState?.OnExit(true);
			ActiveState = state;
			ActiveState?.OnEnter();
		}
	}
}
