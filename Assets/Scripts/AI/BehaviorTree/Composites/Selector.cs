namespace Quinn.AI.BehaviorTree.Composites
{
	public class Selector : Composite
	{
		private int _index;
		private bool _wasSuccess;

		public Selector() { }
		public Selector(params Conditional[] conditionals)
		{
			Conditionals.AddRange(conditionals);
		}
		public Selector(params Service[] services)
		{
			Services.AddRange(services);
		}
		public Selector(Conditional[] conditionals, Service[] services)
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
			// Evaluate higher priority nodes.
			for (int i = 0; i < _index - 1; i++)
			{
				if (Children[i].Evaluate())
				{
					Children[_index].Exit();

					_index = i;
					Children[_index].Enter();

					break;
				}
			}

			// Evaluate current node.
			_wasSuccess = Children[_index].Evaluate();
			if (!_wasSuccess)
			{
				if (!Next())
				{
					return Status.Failure;
				}

				return Status.Running;
			}

			// Update current node.
			var status = Children[_index].Update();

			if (status != Status.Running)
			{
				if (status == Status.Success)
				{
					return Status.Success;
				}
				else if (status == Status.Failure)
				{
					if (!Next())
					{
						return Status.Failure;
					}
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
