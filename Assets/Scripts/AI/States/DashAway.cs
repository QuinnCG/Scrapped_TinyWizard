using UnityEngine;

namespace Quinn.AI.States
{
	public class DashAway : State
	{
		private readonly Transform _target;

		public DashAway(Transform target)
		{
			_target = target;
		}

		protected override void OnEnter()
		{
			Vector2 dir = Agent.Position - (Vector2)_target.position;
			dir.Normalize();

			Agent.Movement.SetDashDirection(dir);
			Agent.Movement.Dash();
		}

		protected override bool OnUpdate()
		{
			return !Agent.Movement.IsDashing;
		}
	}
}
