using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class FaceTarget : Task
	{
		public Transform Target { get; set; }

		public FaceTarget(Transform target)
		{
			Target = target;
		}

		protected override void OnEnter()
		{
			Agent.Movement.SetFacingDirection(Target.position.x - Agent.Position.x);
		}
	}
}
