﻿using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class MoveTowards : Task
	{
		public Transform Target { get; set; }
		public float Speed { get; set; }
		public bool FixedTarget { get; set; }

		private Vector2 _dir;

		public MoveTowards(Transform target, float speed, bool fixedTarget = false)
		{
			Target = target;
			Speed = speed;
			FixedTarget = fixedTarget;
		}

		protected override void OnEnter()
		{
			Agent.Movement.MoveSpeed = Speed;

			if (FixedTarget)
			{
				_dir = ((Vector2)Target.position - Agent.Position).normalized;
			}
		}

		protected override Status OnUpdate()
		{
			Vector2 dir = FixedTarget ? _dir : (Vector2)Target.position - Agent.Position;
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
