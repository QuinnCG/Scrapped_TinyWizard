namespace Quinn.AI.BehaviorTree
{
	public abstract class Conditional
	{
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

		protected virtual void OnEnter() { }
		protected abstract bool OnEvaluate();
		protected virtual void OnExit() { }

		protected virtual void OnParentEnter() { }
		protected virtual void OnParentExit() { }
	}
}
