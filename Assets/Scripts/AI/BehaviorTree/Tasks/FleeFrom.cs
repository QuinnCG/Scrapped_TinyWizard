using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class FleeFrom : Task
	{
		public Transform Target { get; set; }
		public float Speed { get; set; }
		public float Duration { get; set; }
		public bool FleeFromInitialPos { get; set; }

		private float _endTime;
		private Vector2 _initPos;

		public FleeFrom(Transform target, float speed, float duration, bool fleeFromStartPos = false)
		{
			Target = target;
			Speed = speed;
			Duration = duration;
			FleeFromInitialPos = fleeFromStartPos;
		}

		protected override void OnEnter()
		{
			_endTime = Time.time + Duration;
			_initPos = Target.position;

			Agent.Movement.MoveSpeed = Speed;
		}

		protected override Status OnUpdate()
		{
			Vector2 target = FleeFromInitialPos
				? _initPos
				: Target.position;

			Vector2 dir = target - Agent.Position;
			dir.Normalize();

			Agent.Movement.Move(-dir);
			return Time.time > _endTime ? Status.Success : Status.Running;
		}

		protected override void OnExit()
		{
			Agent.Movement.ResetMoveSpeed();
		}
	}
}
