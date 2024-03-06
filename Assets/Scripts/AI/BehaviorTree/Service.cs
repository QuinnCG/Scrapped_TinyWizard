namespace Quinn.AI.BehaviorTree
{
	public abstract class Service
	{
		protected Enemy Agent { get; private set; }

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

		public void SetAgent(Enemy enemy)
		{
			Agent = enemy;
		}

		protected virtual void OnEnter() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnExit() { }

		protected virtual void OnParentEnter() { }
		protected virtual void OnParentExit() { }
	}
}
