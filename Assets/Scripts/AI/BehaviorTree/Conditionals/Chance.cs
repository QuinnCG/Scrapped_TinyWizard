using Unity.Mathematics;

namespace Quinn.AI.Conditionals
{
	public class Chance : Conditional
	{
		private static Random _rand = new((uint)System.DateTime.Now.Ticks);

		public float Percent { get; set; }

		private bool _success;

		public Chance(float percent)
		{
			Percent = percent;
		}

		protected override void OnParentEnter()
		{
			_success = _rand.NextFloat() <= Percent;
		}

		protected override bool OnEvaluate()
		{
			return _success;
		}
	}
}
