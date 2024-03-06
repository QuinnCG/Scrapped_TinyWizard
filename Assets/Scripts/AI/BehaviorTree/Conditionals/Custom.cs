using System;

namespace Quinn.AI.Conditionals
{
	public class Custom : Conditional
	{
		private readonly Func<bool> _condition;

		public Custom(Func<bool> condition)
		{
			_condition = condition;
		}

		protected override bool OnEvaluate()
		{
			return _condition();
		}
	}
}
