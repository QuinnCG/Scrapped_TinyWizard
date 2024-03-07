using System;

namespace Quinn.AI.Services
{
	public class Custom : Service
	{
		public Action Action { get; set; }

		public Custom(Action update)
		{
			Action = update;
		}

		protected override void OnUpdate()
		{
			Action();
		}
	}
}
