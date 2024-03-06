namespace Quinn.AI.Conditionals
{
	public class Trigger : Conditional
	{
		private bool _triggered;

		public void Set()
		{
			_triggered = true;
		}

		public void Reset()
		{
			_triggered = false;
		}

		protected override bool OnEvaluate()
		{
			return _triggered;
		}

		protected override void OnExit()
		{
			_triggered = false;
		}
	}
}
