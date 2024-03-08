using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class TriggerAnim : Task
	{
		public string Key { get; set; }

		private float _endTime;

		public TriggerAnim(string key)
		{
			Key = key;
		}

		public override string ToString() => Key;

		protected override void OnEnter()
		{
			Agent.Animator.SetTrigger(Key);
			float duration = Agent.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

			_endTime = Time.time + duration;
		}

		protected override Status OnUpdate()
		{
			return Time.time > _endTime ? Status.Success : Status.Running;
		}
	}
}
