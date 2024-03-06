using UnityEngine;

namespace Quinn.AI.Conditionals
{
	public class FarFrom : Conditional
	{
		public Transform Target { get; set; }
		public float Distance { get; set; }

		public FarFrom(Transform target, float distance)
		{
			Target = target;
			Distance = distance;
		}

		protected override bool OnEvaluate()
		{
			float dst = Vector2.Distance(Agent.Position, Target.position);
			return dst > Distance;
		}
	}
}
