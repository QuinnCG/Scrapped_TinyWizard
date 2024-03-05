using UnityEngine;

namespace Quinn.AI.States
{
	public class Wait : State
	{
		private float _endTime;

		private readonly bool _isRandom;
		private readonly float _min, _max;

		public Wait(float duration)
		{
			_endTime = Time.time + duration;
		}
		public Wait(float minDuration, float maxDuration)
		{
			_isRandom = true;
			_min = minDuration;
			_max = maxDuration;
		}

		protected override void OnEnter()
		{
			if (_isRandom)
			{
				_endTime = Time.time + Random.Range(_min, _max);
			}
		}

		protected override bool OnUpdate()
		{
			return Time.time >= _endTime;
		}
	}
}
