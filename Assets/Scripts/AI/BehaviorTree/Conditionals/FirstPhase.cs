namespace Quinn.AI.Conditionals
{
	public class FirstPhase : Conditional
	{
		protected override bool OnEvaluate()
		{
			return !Agent.IsHalfHealth;
		}
	}
}
