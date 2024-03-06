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

		protected virtual void OnEnter() { }
		protected abstract bool OnEvaluate();
		protected virtual void OnExit() { }
	}
}
