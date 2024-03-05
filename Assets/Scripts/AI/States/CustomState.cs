using System;

namespace Quinn.AI.States
{
	public class CustomState : State
	{
		private readonly Action _enter, _exit;
		private readonly Func<bool> _update;

		public CustomState(Func<bool> update)
		{
			_update = update;
		}
		public CustomState(Action update)
		{
			_update += () => { update(); return false; };
		}
		public CustomState(Action enter = null, Action update = null, Action exit = null)
		{
			_enter = enter;
			_update += () => { update(); return false; };
			_exit = exit;
		}
		public CustomState(Action enter = null, Func<bool> update = null, Action exit = null)
		{
			_enter = enter;
			_update += update;
			_exit = exit;
		}

		public static CustomState Enter(Action enter)
		{
			return new CustomState(enter, null, null);
		}

		protected override void OnEnter()
		{
			_enter?.Invoke();
		}

		protected override bool OnUpdate()
		{
			bool? result = _update?.Invoke();
			return result != null;
		}

		protected override void OnExit()
		{
			_exit?.Invoke();
		}
	}
}
