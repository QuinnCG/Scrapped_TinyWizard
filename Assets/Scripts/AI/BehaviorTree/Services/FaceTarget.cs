using UnityEngine;

namespace Quinn.AI.Services
{
	public class FaceTarget : Service
	{
		private readonly Transform _target;

		public FaceTarget(Transform target)
		{
			_target = target;
		}

		protected override void OnUpdate()
		{
			Vector2 dir = (Vector2)_target.position - Agent.Position;
			dir.Normalize();

			Agent.Movement.SetMoveDirection(dir);
		}
	}
}
