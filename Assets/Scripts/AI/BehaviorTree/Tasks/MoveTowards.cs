using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class MoveTowards : Task
	{
		public Transform Target { get; set; }
		public float Speed { get; set; }

		private bool _fixedTarget;
		private Vector2 _target;

		public MoveTowards(Transform target, float speed, bool fixTarget = false)
		{
			Target = target;
			Speed = speed;
			_fixedTarget = fixTarget;
		}

		protected override void OnEnter()
		{
			Agent.Movement.MoveSpeed = Speed;

			if (_fixedTarget)
			{
				_target = Target.position;
			}
		}

		protected override Status OnUpdate()
		{
			Vector2 target = _fixedTarget ? _target : Target.position;

			Vector2 dir = target - Agent.Position;
			dir.Normalize();
			Agent.Movement.Move(dir);

			return Status.Running;
		}

		protected override void OnExit()
		{
			Agent.Movement.ResetMoveSpeed();
		}
	}
}
