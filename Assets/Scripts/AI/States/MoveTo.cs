using UnityEngine;

namespace Quinn.AI.States
{
	public partial class MoveTo : State
	{
		public const float DefaultStoppingDistance = 1f;

		private readonly Transform _target;
		private readonly float _endTime;
		private readonly float _stoppingDistance;

		public MoveTo(Transform target, float timeout = float.PositiveInfinity, float stoppingDistance = DefaultStoppingDistance)
		{
			_target = target;
			_endTime = Time.time + timeout;
			_stoppingDistance = stoppingDistance;
		}

		protected override bool OnUpdate()
		{
			Agent.Movement.MoveTowards(_target.position);

			float dst = Vector2.Distance(Agent.Position, _target.position);
			return  dst <= _stoppingDistance || Time.time >= _endTime;
		}
	}
}
