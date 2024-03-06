using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class MoveTowards : Task
	{
		public Transform Target { get; set; }
		public float Speed { get; set; }

		public MoveTowards(Transform target, float speed)
		{
			Target = target;
			Speed = speed;
		}

		protected override void OnEnter()
		{
			Agent.Movement.MoveSpeed = Speed;
		}

		protected override Status OnUpdate()
		{
			Vector2 dir = (Vector2)Target.position - Agent.Position;
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
