using UnityEngine;

namespace Quinn.AI.States
{
	public class PlayAnimation : State
	{
		private readonly string _key;
		private readonly bool? _value;
		private float _endTime;

		public PlayAnimation(string trigger)
		{
			_key = trigger;
			_value = null;
		}
		public PlayAnimation(string flag, bool value)
		{
			_key = flag;
			_value = value;
		}

		protected override void OnEnter()
		{
			if (_value.HasValue)
			{
				Agent.Animator.SetBool(_key, _value.Value);
			}
			else
			{
				Agent.Animator.SetTrigger(_key);

				float dur = Agent.Animator.GetCurrentAnimatorStateInfo(0).length;
				_endTime = Time.time + dur;
			}
		}

		protected override bool OnUpdate()
		{
			if (_value.HasValue) return false;
			return Time.time >= _endTime;
		}
	}
}
