namespace Quinn.AI
{
	public abstract class State
	{
		public Enemy Agent { protected get; set; }
		public bool IsInterruptable { get; protected set; } = true;

		public virtual void OnEnter() { }
		public virtual bool OnUpdate() => false;
		public virtual void OnExit(bool isInterruption) { }
	}
}
