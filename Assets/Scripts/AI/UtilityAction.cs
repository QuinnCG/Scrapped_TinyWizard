using System.Collections.Generic;

namespace Quinn.AI
{
	public class UtilityAction
	{
		public State State;
		public readonly List<(Meter meter, float delta)> Deltas = new();

		public float CalculateValue()
		{
			float value = 0f;

			foreach (var delta in Deltas)
			{
				value += delta.meter.Factor * delta.delta;
			}

			return value;
		}
	}
}
