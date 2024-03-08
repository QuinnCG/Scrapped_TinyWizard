using System;

namespace Quinn.AI.Services
{
	public class Custom : Service
	{
		public new Action Enter { get; set; }
		public new Action Update { get; set; }
		public new Action Exit { get; set; }

		public Custom(Action update)
		{
			Update = update;
		}
		public Custom(Action enter, Action exit)
		{
			Enter = enter;
			Exit = exit;
		}
		public Custom(Action enter, Action update, Action exit = null)
		{
			Enter = enter;
			Update = update;
			Exit = exit;
		}

		protected override void OnEnter()
		{
			Enter?.Invoke();
		}

		protected override void OnUpdate()
		{
			Update?.Invoke();
		}

		protected override void OnExit()
		{
			Exit?.Invoke();
		}
	}
}
