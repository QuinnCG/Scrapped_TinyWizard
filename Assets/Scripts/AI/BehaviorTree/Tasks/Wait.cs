using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class Wait : Task
	{
		private readonly float _duration;
		private readonly float _deviation;

		private float _endTime;

		public Wait(float duration, float deviation = 0f)
		{
			_duration = duration;
			_deviation = deviation;
		}

		protected override void OnEnter()
		{
			_endTime = Time.time + _duration + Random.Range(-_deviation, _deviation);
		}

		protected override Status OnUpdate()
		{
			return Time.time > _endTime ? Status.Success : Status.Running;
		}
	}
}
