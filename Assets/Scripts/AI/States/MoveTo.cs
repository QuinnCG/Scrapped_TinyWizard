using System.Threading;
using UnityEngine;

namespace Quinn.AI.States
{
	public class MoveTo : State
	{
		public const float DefaultStoppingDistance = 1f;

		private readonly Transform _target;
		private readonly float _timeout;
		private readonly float _stoppingDistance;

		private float _endTime;

		public MoveTo(Transform target, float timeout = float.PositiveInfinity, float stoppingDistance = DefaultStoppingDistance)
		{
			_target = target;
			_timeout = timeout;
			_stoppingDistance = stoppingDistance;
		}

		protected override void OnEnter()
		{
			_endTime = Time.time + _timeout;
		}

		protected override bool OnUpdate()
		{
			Agent.Movement.MoveTowards(_target.position);
			float dst = Vector2.Distance(Agent.Position, _target.position);

			return  dst <= _stoppingDistance || Time.time >= _endTime;
		}
	}
}
