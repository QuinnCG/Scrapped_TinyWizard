using UnityEngine;

namespace Quinn.AI.BehaviorTree.Tasks
{
	public class Log : Task
	{
		private readonly string _message;

		public Log(string message)
		{
			_message = message;
		}

		protected override void OnEnter()
		{
			Debug.Log(_message);
		}
	}
}
