namespace Quinn.AI
{
	public abstract class Conditional
	{
		protected Composite Parent { get; private set; }
		protected Enemy Agent => Parent.Agent;

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

		public void SetParent(Composite parent)
		{
			Parent = parent;
		}

		protected virtual void OnEnter() { }
		protected abstract bool OnEvaluate();
		protected virtual void OnExit() { }

		protected virtual void OnParentEnter() { }
		protected virtual void OnParentExit() { }
	}
}
