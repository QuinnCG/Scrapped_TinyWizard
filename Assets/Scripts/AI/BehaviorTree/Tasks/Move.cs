namespace Quinn.AI.Tasks
{
	public class Move : Task
	{
		public float Speed { get; set; }

		public Move(float speed)
		{
			Speed = speed;
		}

		protected override void OnEnter()
		{
			Agent.Movement.MoveSpeed = Speed;
		}

		protected override Status OnUpdate()
		{
			Agent.Movement.Move();
			return Status.Running;
		}

		protected override void OnExit()
		{
			Agent.Movement.ResetMoveSpeed();
		}
	}
}
