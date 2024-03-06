using System;

namespace Quinn.AI.Conditionals
{
	public class Custom : Conditional
	{
		public Func<bool> Condition { get; set; }

		public Custom(Func<bool> condition)
		{
			Condition = condition;
		}

		protected override bool OnEvaluate()
		{
			return Condition();
		}
	}
}
