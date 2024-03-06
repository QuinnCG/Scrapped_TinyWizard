namespace Quinn.AI
{
	public abstract class Conditional
	{
		protected Enemy Agent { get; private set; }

		public void Enter()
		{
			OnEnter();
		}

		public bool Evaluate()
		{
			return OnEvaluate();
		}

		public void Exit()
		{
			OnExit();
		}

		public void ParentEnter()
		{
			OnParentEnter();
		}

		public void ParentExit()
		{
			OnParentExit();
		}

		public void SetAgent(Enemy enemy)
		{
			Agent = enemy;
		}

		protected virtual void OnEnter() { }
		protected abstract bool OnEvaluate();
		protected virtual void OnExit() { }

		protected virtual void OnParentEnter() { }
		protected virtual void OnParentExit() { }
	}
}
