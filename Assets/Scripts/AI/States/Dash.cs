using UnityEngine;

namespace Quinn.AI.States
{
	public class Dash : State
	{
		private readonly Transform _target;

		public Dash(Transform target)
		{
			_target = target;
		}

		protected override void OnEnter()
		{
			Vector2 dir = (Vector2)_target.position - Agent.Position;
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
