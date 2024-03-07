namespace Quinn.AI.Composites
{
	public class Parallel : Composite
	{
		public Parallel() { }
		public Parallel(params Conditional[] conditionals)
		{
			Conditionals.AddRange(conditionals);
		}
		public Parallel(params Service[] services)
		{
			Services.AddRange(services);
		}
		public Parallel(Conditional[] conditionals, Service[] services)
		{
			Conditionals.AddRange(conditionals);
			Services.AddRange(services);
		}

		protected override void OnEnter()
		{
			foreach (var child in Children)
			{
				child.Enter();
			}
		}

		protected override Status OnUpdate()
		{
			bool wasRunning = false, didFail = false;

			foreach (var child in Children)
			{
				var status = child.Update();

				if (status == Status.Running) wasRunning = true;
				if (status == Status.Failure) didFail = true;
			}

			if (didFail)
			{
				Children[0].Exit();
				return Status.Failure;
			}

			if (wasRunning) return Status.Running;

			Children[0].Exit();
			return Status.Success;
		}

		public override void Exit()
		{
			foreach (var child in Children)
			{
				child.Exit();
			}
		}
	}
}
