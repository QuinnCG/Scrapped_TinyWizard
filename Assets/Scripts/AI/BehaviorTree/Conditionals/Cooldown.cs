using UnityEngine;

namespace Quinn.AI.Conditionals
{
	public class Cooldown : Conditional
	{
		public float Duration { get; set; }
		public float Deviation { get; set; }

		private float _endTime;

		public Cooldown(float duration, float deviation = 0f)
		{
			Duration = duration;
			Deviation = deviation;
		}
		public Cooldown(AIRand rand)
		{
			Duration = rand.Mean;
			Deviation = rand.Mean;
		}

		protected override void OnEnter()
		{
			_endTime = Time.time + Duration + Random.Range(-Deviation, Deviation);
		}

		protected override bool OnEvaluate()
		{
			return Time.time > _endTime;
		}
	}
}
