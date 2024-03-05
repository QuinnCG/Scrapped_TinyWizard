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
			OnEnter();
		}

		public bool Update()
		{
			return OnUpdate();
		}

		public void Exit()
		{
			OnExit();
		}

		protected virtual void OnEnter() { }
		protected virtual bool OnUpdate() => true;
		protected virtual void OnExit() { }
	}
}
