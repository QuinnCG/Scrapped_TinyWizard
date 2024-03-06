namespace Quinn.AI.BehaviorTree.Composites
{
	public class Sequence : Composite
	{
		private int _index;
		private bool _wasSuccess;

		public Sequence() { }
		public Sequence(params Conditional[] conditionals)
		{
			Conditionals.AddRange(conditionals);
		}
		public Sequence(params Service[] services)
		{
			Services.AddRange(services);
		}
		public Sequence(Conditional[] conditionals, Service[] services)
		{
			Conditionals.AddRange(conditionals);
			Services.AddRange(services);
		}

		protected override void OnEnter()
		{
			_index = 0;

			if (Children[_index].Evaluate())
			{
				Children[_index].Enter();
			}
		}

		protected override Status OnUpdate()
		{
			// Evaluate current node.
			_wasSuccess = Children[_index].Evaluate();
			if (!_wasSuccess)
			{
				return Status.Failure;
			}

			// Update current node.
			var status = Children[_index].Update();

			if (status != Status.Running)
			{
				if (status == Status.Success)
				{
					if (!Next())
					{
						return Status.Success;
					}
				}
				else if (status == Status.Failure)
				{
					return Status.Failure;
				}
			}

			return Status.Running;
		}

		protected override void OnExit()
		{
			if (_wasSuccess)
			{
				Children[_index].Exit();
			}
		}

		private bool Next()
		{
			Children[_index].Exit();

			_index++;

			if (_index >= Children.Count)
			{
				_wasSuccess = false;
				return false;
			}

			Children[_index].Enter();

			return true;
		}
	}
}
