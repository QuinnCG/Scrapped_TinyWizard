using UnityEngine;

namespace Quinn.AI.Conditionals
{
	public class Timer : Conditional
	{
		public AIRand Duration { get; set; }

		private float _endTime;

		public Timer(AIRand duration)
		{
			Duration = duration;
		}
		public Timer(float duration, float deviation = 0f)
		{
			Duration = new(duration, deviation);
		}

		protected override void OnEnter()
		{
			_endTime = Time.time + Duration.Mean + Random.Range(-Duration.Deviation, Duration.Deviation);
		}

		protected override bool OnEvaluate()
		{
			return Time.time > _endTime;
		}
	}
}
