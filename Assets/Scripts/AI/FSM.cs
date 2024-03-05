using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn.AI
{
	public class FSM
	{
		public State Current { get; private set; }
		public bool DebugMode { get; set; }

		protected Enemy Agent { get; }

		private readonly HashSet<State> _states = new();

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

		public void Update()
		{
			bool wantsToExit = Current.Update();

			foreach (var (to, condition) in Current.Connections)
			{
				if (condition(wantsToExit))
				{
					TransitionTo(to);
					break;
				}
			}
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

		public void SetStart(State start)
		{
			Current ??= start;
			Current?.SetAgent(Agent);
		}

		private void TransitionTo(State state)
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
			}
		}

		private void Print(string message)
		{
			Debug.Log($"<color=white><b>[FSM]:</b> {message}</color>");
		}
	}
}
