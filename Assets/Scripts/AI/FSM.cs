using System.Collections.Generic;
using System.Linq;

namespace Quinn.AI
{
	public class FSM
	{
		public State Current { get; private set; }

		private readonly HashSet<State> _states = new();

		public void Update()
		{
			if (Current != null)
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
			else
			{
				Current = _states.FirstOrDefault();
			}
		}

		public void Connect(State from, State to, Condition condition)
		{
			if (!_states.Contains(from))
			{
				_states.Add(from);
			}

			from.Connect(to, condition);
		}

		public void SetStart(State start)
		{
			Current ??= start;
		}

		private void TransitionTo(State state)
		{
			if (state != Current)
			{
				Current?.Exit();
				Current = state;
				Current?.Enter();
			}
		}
	}
}
