using Quinn.AI.Composites;
using System.Collections;
using System.Collections.Generic;

namespace Quinn.AI
{
	public class Tree : IEnumerable<Node>, IEnumerable
	{
		public Enemy Agent { get; private set; }
		public Selector Root { get; }
		public bool DebugMode { get; set; }

		private bool _exited = true;

		public Tree()
		{
			Root = new Selector();
			Root.SetTree(this);
		}

		public void Update()
		{
			if (_exited)
			{
				Root.Enter();
				_exited = false;
			}

			var status = Root.Update();

			if (status != Status.Running)
			{
				Root.Exit();
				_exited = true;
			}
		}

		public void Add(Node node)
		{
			Root.Add(node);
		}

		public void SetAgent(Enemy agent)
		{
			Agent = agent;
		}

		public T GetChild<T>(string name = "") where T : Node
		{
			return Root.GetChild<T>(name);
		}

		public IEnumerator<Node> GetEnumerator()
		{
			return Root.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
