using UnityEngine;

namespace Quinn.AI.Services
{
	public class MoveTowards : Service
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

		protected override void OnUpdate()
		{
			Vector2 dir = (Vector2)Target.position - Agent.Position;
			dir.Normalize();
			Agent.Movement.Move(dir);
		}

		protected override void OnExit()
		{
			Agent.Movement.ResetMoveSpeed();
		}
	}
}
