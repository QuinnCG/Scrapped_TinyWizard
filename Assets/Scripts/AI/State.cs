using System;
using System.Collections.Generic;
using System.Linq;

namespace Quinn.AI
{
	public abstract class State
	{
		public IEnumerable<(State to, Condition condition)> Connections => _connections.AsEnumerable();

		protected Enemy Agent { get; private set; }

		private readonly List<(State to, Condition condition)> _connections = new();

		private Action _enterCallback, _updateCallback, _exitCallback;

		public void Connect(State to, Condition condition)
		{
			_connections.Add((to, condition));
		}

		public void SetAgent(Enemy agent)
		{
			Agent = agent;
		}

		public void Enter()
		{
			_enterCallback?.Invoke();
			OnEnter();
		}

		public bool Update()
		{
			_updateCallback?.Invoke();
			return OnUpdate();
		}

		public void Exit()
		{
			_exitCallback?.Invoke();
			OnExit();
		}

		public State AddEnter(Action callback)
		{
			_enterCallback += callback;
			return this;
		}

		public State AddUpdate(Action callback)
		{
			_updateCallback += callback;
			return this;
		}

		public State AddExit(Action callback)
		{
			_exitCallback += callback;
			return this;
		}

		protected virtual void OnEnter() { }
		protected virtual bool OnUpdate() => true;
		protected virtual void OnExit() { }
	}
}
