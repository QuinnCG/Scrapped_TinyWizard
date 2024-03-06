namespace Quinn.AI.Composites
{
	public class Fail : Composite
	{
		public Fail() { }
		public Fail(params Conditional[] conditionals)
		{
			Conditionals.AddRange(conditionals);
		}
		public Fail(params Service[] services)
		{
			Services.AddRange(services);
		}
		public Fail(Conditional[] conditionals, Service[] services)
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
				return Status.Failure;
			}

			return Status.Running;
		}

		public override void Exit()
		{
			Children[0].Exit();
		}
	}
}
