using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class MoveTo : Task
	{
		public Transform Target { get; set; }
		public float Speed { get; set; }
		public float StoppingDistance { get; set; }
		public float Timeout { get; set; }

		private float _timeoutTime;

		public MoveTo(Transform target, float speed, float stoppingDistance = 0.5f, float timeout = float.PositiveInfinity)
		{
			Target = target;
			Speed = speed;
			StoppingDistance = stoppingDistance;
			Timeout = timeout;
		}

		protected override void OnEnter()
		{
			Agent.Movement.MoveSpeed = Speed;
			_timeoutTime = Time.time + Timeout;
		}

		protected override Status OnUpdate()
		{
			Vector2 dir = (Vector2)Target.position - Agent.Position;
			dir.Normalize();
			Agent.Movement.Move(dir);

			if (Time.time > _timeoutTime)
			{
				return Status.Failure;
			}

			float dst = Vector2.Distance(Agent.Position, Target.position);
			return dst < StoppingDistance ? Status.Success : Status.Running;
		}

		protected override void OnExit()
		{
			Agent.Movement.ResetMoveSpeed();
		}
	}
}
