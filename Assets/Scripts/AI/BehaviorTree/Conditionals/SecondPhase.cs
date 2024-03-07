namespace Quinn.AI.Conditionals
{
	public class SecondPhase : Conditional
	{
		protected override bool OnEvaluate()
		{
			return Agent.IsHalfHealth;
		}
	}
}
