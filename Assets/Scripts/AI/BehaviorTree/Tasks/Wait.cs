using DG.Tweening;
using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class Wait : Task
	{
		public float Duration { get; set; }
		public float Deviation { get; set; }

		private float _endTime;

		public Wait(float duration, float deviation = 0f)
		{
			Duration = duration;
			Deviation = deviation;
		}
		public Wait(AIRand rand)
		{
			Duration = rand.Mean;
			Deviation = rand.Deviation;
		}

		protected override void OnEnter()
		{
			_endTime = Time.time + Duration + Random.Range(-Deviation, Deviation);
		}

		protected override Status OnUpdate()
		{
			return Time.time > _endTime ? Status.Success : Status.Running;
		}
	}
}
