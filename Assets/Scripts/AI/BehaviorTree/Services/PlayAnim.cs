namespace Quinn.AI.BT.Services
{
	public class PlayAnim : Service
	{
		private readonly string _key;

		public PlayAnim(string key)
		{
			_key = key;
		}

		protected override void OnEnter()
		{
			Agent.Animator.SetBool(_key, true);
		}

		protected override void OnExit()
		{
			Agent.Animator.SetBool(_key, false);
		}
	}
}
