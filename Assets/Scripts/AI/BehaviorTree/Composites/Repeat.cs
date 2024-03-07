using UnityEngine;

namespace Quinn.AI.Composites
{
	public class Repeat : Composite
	{
		public int Min { get; set; } = 1;
		public int Max { get; set; } = 1;

		private int _remaining;

		public Repeat(int min)
		{
			Min = min;
		}
		public Repeat(int min, int max)
		{
			Min = min;
			Max = max;
		}
		public Repeat(int min, int max, params Conditional[] conditionals)
		{
			Min = min;
			Max = max;

			Conditionals.AddRange(conditionals);
		}
		public Repeat(int min, int max, params Service[] services)
		{
			Min = min;
			Max = max;

			Services.AddRange(services);
		}
		public Repeat(int min, int max, Conditional[] conditionals, Service[] services)
		{
			Min = min;
			Max = max;

			Conditionals.AddRange(conditionals);
			Services.AddRange(services);
		}

		protected override void OnEnter()
		{
			_remaining = Random.Range(Min, Max + 1);
			Children[0].Enter();
		}

		protected override Status OnUpdate()
		{
			var status = Children[0].Update();

			if (status == Status.Success)
			{
				_remaining--;
				Children[0].Exit();
				Children[0].Enter();
			}

			if (_remaining <= 0)
			{
				return Status.Success;
			}

			if (status == Status.Failure)
			{
				return Status.Failure;
			}

			return Status.Running;
		}

		protected override void OnExit()
		{
			Children[0].Exit();
		}
	}
}
