using UnityEngine;

namespace Quinn.AI.Conditionals
{
	public class Cooldown : Conditional
	{
		private readonly float _duration;
		private readonly float _deviation;

		private float _endTime;

		public Cooldown(float duration, float deviation = 0f)
		{
			_duration = duration;
			_deviation = deviation;
		}

		protected override void OnEnter()
		{
			_endTime = Time.time + _duration + Random.Range(-_deviation, _deviation);
		}

		protected override bool OnEvaluate()
		{
			return Time.time > _endTime;
		}
	}
}
