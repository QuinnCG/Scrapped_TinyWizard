using UnityEngine;

namespace Quinn.AI.States
{
	public class Flee : State
	{
		private readonly Transform _from;
		private readonly float _stopTime;

		public Flee(Transform from, float duration)
		{
			_from = from;
			_stopTime = Time.time + duration;
		}

		protected override bool OnUpdate()
		{
			Vector2 dir = (Vector2)_from.position - Agent.Position;
			dir.Normalize();

			Agent.Movement.Move(-dir);
			return Time.time > _stopTime;
		}
	}
}
