using System;

namespace Quinn.AI.Tasks
{
	public class Custom : Task
	{
		public new Action Enter { get; set; }
		public new Func<Status> Update { get; set; }
		public new Action Exit { get; set; }

		public Custom(Func<Status> update)
		{
			Update = update;
		}
		public Custom(Action enter, Action exit)
		{
			Enter = enter;
			Exit = exit;
		}
		public Custom(Action enter, Func<Status> update, Action exit = null)
		{
			Enter = enter;
			Update = update;
			Exit = exit;
		}

		protected override void OnEnter()
		{
			Enter?.Invoke();
		}

		protected override Status OnUpdate()
		{
			Status? status = Update?.Invoke();
			if (status == null) return Status.Failure;
			return (Status)status;
		}

		protected override void OnExit()
		{
			Exit?.Invoke();
		}
	}
}
