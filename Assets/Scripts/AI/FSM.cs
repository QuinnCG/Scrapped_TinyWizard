using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn.AI
{
	public class FSM
	{
		public State Current { get; private set; }
		public bool DebugMode { get; set; }

		public event Action<State> OnWantToExit;
		public event Action<State> OnTransition;
		public event Action<State> OnUpdate;

		protected Enemy Agent { get; }

		private readonly HashSet<State> _states = new();

		private State _startState;
		private bool _wantedToExit;

		public FSM(Enemy agent)
		{
			Agent = agent;
		}

		public void Start()
		{
			if (DebugMode)
			{
				Print($"Starting state: {Current?.GetType().Name}!");
			}
		}

		public bool Update()
		{
			if (Current == null && _startState != null)
			{
				TransitionTo(_startState);
			}
			else if (_startState == null && Current == null)
			{
				return true;
			}

			bool wantsToExit = Current.Update();
			OnUpdate?.Invoke(Current);

			if (wantsToExit && !_wantedToExit)
			{
				_wantedToExit = true;
				OnWantToExit?.Invoke(Current);
			}

			foreach (var (to, condition) in Current.Connections)
			{
				if (condition(wantsToExit))
				{
					TransitionTo(to);
					break;
				}
			}

			return wantsToExit;
		}

		public void Connect(State from, State to, Condition condition)
		{
			if (!_states.Contains(from))
			{
				_states.Add(from);
			}

			from.Connect(to, condition);

			from.SetAgent(Agent);
			to.SetAgent(Agent);
		}

		public void Register(State state)
		{
			_states.Add(state);
			state.SetAgent(Agent);
		}

		public void SetStart(State start)
		{
			_startState = start;
			start.SetAgent(Agent);
		}

		public void TransitionTo(State state)
		{
			if (state != Current)
			{
				if (DebugMode)
				{
					Print($"{Current?.GetType().Name}: Exited!");
				}

				Current?.Exit();
				Current = state;
				Current?.Enter();

				if (DebugMode)
				{
					Print($"{Current?.GetType().Name}: Entered!");
				}

				_wantedToExit = false;

				OnTransition?.Invoke(state);
			}
		}

		private void Print(string message)
		{
			Debug.Log($"<color=white><b>[FSM]:</b> {message}</color>");
		}
	}
}
