namespace Quinn.AI.Tasks
{
	public class TriggerAnim : Task
	{
		private readonly string _key;

		public TriggerAnim(string key)
		{
			_key = key;
		}

		protected override void OnEnter()
		{
			Agent.Animator.SetTrigger(_key);
		}
	}
}
