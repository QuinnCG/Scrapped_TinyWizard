using UnityEngine;

namespace Quinn.AI.States
{
	public partial class MoveTo : State
	{
		public const float DefaultStoppingDistance = 0.2f;

		private readonly Transform _target;
		private readonly float _stoppingDistance;

		public MoveTo(Transform target, float stoppingDistance = DefaultStoppingDistance)
		{
			_target = target;
			_stoppingDistance = stoppingDistance;
		}

		protected override bool OnUpdate()
		{
			Agent.Movement.MoveTowards(_target.position);

			float dst = Vector2.Distance(Agent.Position, _target.position);
			return  dst <= _stoppingDistance;
		}
	}
}
