namespace Quinn.AI.Composites
{
	public class Succeed : Composite
	{
		public Succeed() { }
		public Succeed(params Conditional[] conditionals)
		{
			Conditionals.AddRange(conditionals);
		}
		public Succeed(params Service[] services)
		{
			Services.AddRange(services);
		}
		public Succeed(Conditional[] conditionals, Service[] services)
		{
			Conditionals.AddRange(conditionals);
			Services.AddRange(services);
		}

		protected override void OnEnter()
		{
			Children[0].Enter();
		}

		protected override Status OnUpdate()
		{
			var status = Children[0].Update();

			if (status != Status.Running)
			{
				Children[0].Exit();
				return Status.Success;
			}

			return Status.Running;
		}

		public override void Exit()
		{
			Children[0].Exit();
		}
	}
}
