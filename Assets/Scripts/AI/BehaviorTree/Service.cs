namespace Quinn.AI
{
	public abstract class Service
	{
		protected Composite Parent { get; private set; }
		protected Enemy Agent => Parent.Agent;

		public void Enter()
		{
			OnEnter();
		}

		public void Update()
		{
			OnUpdate();
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
		protected virtual void OnUpdate() { }
		protected virtual void OnExit() { }

		protected virtual void OnParentEnter() { }
		protected virtual void OnParentExit() { }
	}
}
