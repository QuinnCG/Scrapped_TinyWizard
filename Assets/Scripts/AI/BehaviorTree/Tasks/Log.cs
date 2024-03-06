using UnityEngine;

namespace Quinn.AI.Tasks
{
	public class Log : Task
	{
		public string Message { get; set; }

		public Log(string message)
		{
			Message = message;
		}

		protected override void OnEnter()
		{
			Debug.Log(Message);
		}
	}
}
