using System.Linq;

namespace Quinn.AI.States
{
	public class StateSequence : State
	{
		private readonly FSM _fsm;
		private readonly State _last;

		public StateSequence(Enemy agent, params State[] states)
		{
			_fsm = new FSM(agent);
			_last = states.Last();

			if (states.Length > 0)
			{
				_fsm.SetStart(states[0]);

				for (int i = 0; i < states.Length - 1; i++)
				{
					_fsm.Connect(states[i], states[i + 1], exit => exit);
				}
			}
		}

		protected override void OnEnter()
		{
			_fsm.Start();
		}

		protected override bool OnUpdate()
		{
			bool exit = _fsm.Update();
			return exit && _fsm.Current == _last;
		}

		protected override void OnExit()
		{
			_fsm.TransitionTo(null);
		}
	}
}
