using UnityEngine.TextCore.Text;

namespace Quinn.AI.Composites
{
	public class Invert : Composite
	{
		public Invert() { }
		public Invert(params Conditional[] conditionals)
		{
			Conditionals.AddRange(conditionals);
		}
		public Invert(params Service[] services)
		{
			Services.AddRange(services);
		}
		public Invert(Conditional[] conditionals, Service[] services)
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
				if (status == Status.Success) return Status.Failure;
				if (status == Status.Failure) return Status.Success;
			}

			return Status.Running;
		}

		public override void Exit()
		{
			Children[0].Exit();
		}
	}
}
