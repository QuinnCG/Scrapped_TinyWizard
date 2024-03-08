using UnityEngine;

namespace Quinn.AI.Conditionals
{
	public class Cooldown : Conditional
	{
		public float Duration { get; set; }
		public float Deviation { get; set; }
		public bool StartOnCooldown { get; set; }

		private float _endTime;
		private bool _isFirstEnter = true;

		public Cooldown(float duration, float deviation = 0f, bool startOnCooldown = false)
		{
			Duration = duration;
			Deviation = deviation;

			StartOnCooldown = startOnCooldown;
		}
		public Cooldown(AIRand rand, bool startOnCooldown = false)
		{
			Duration = rand.Mean;
			Deviation = rand.Mean;

			StartOnCooldown = startOnCooldown;

		}

		protected override void OnEnter()
		{
			if (_isFirstEnter)
			{
				_isFirstEnter = false;
				OnExit();
			}
		}

		protected override bool OnEvaluate()
		{
			return Time.time > _endTime;
		}

		protected override void OnExit()
		{
			_endTime = Time.time + Duration + Random.Range(-Deviation, Deviation);
		}
	}
}
