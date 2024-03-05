namespace Quinn.AI.States
{
	public class SubFSM : State
	{
		private readonly FSM _fsm;

		public SubFSM(FSM fsm)
		{
			_fsm = fsm;
		}

		protected override void OnEnter()
		{
			_fsm.Start();
		}

		protected override bool OnUpdate()
		{
			return _fsm.Update();
		}

		protected override void OnExit()
		{
			_fsm.TransitionTo(null);
		}
	}
}
