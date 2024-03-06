using Unity.Mathematics;

namespace Quinn.AI.BehaviorTree.Conditionals
{
	public class Chance : Conditional
	{
		private static Random _rand = new((uint)System.DateTime.Now.Ticks);

		private readonly float _chance;
		private bool _success;

		public Chance(float chance)
		{
			_chance = chance;
		}

		protected override void OnParentEnter()
		{
			_success = _rand.NextFloat() <= _chance;
		}

		protected override bool OnEvaluate()
		{
			return _success;
		}
	}
}
