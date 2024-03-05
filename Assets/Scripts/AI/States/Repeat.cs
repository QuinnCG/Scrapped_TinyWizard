namespace Quinn.AI.States
{
	public class Repeat : State
	{
		private readonly State _state;

		public Repeat(State state)
		{
			_state = state;
		}

		protected override void OnEnter()
		{
			_state.Enter();
		}

		protected override bool OnUpdate()
		{
			if (_state.Update())
			{
				_state.Exit();
				_state.Enter();
			}

			return false;
		}

		protected override void OnExit()
		{
			_state.Exit();
		}
	}
}
