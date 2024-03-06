using Quinn.AI.BehaviorTree.Composites;
using System.Collections;
using System.Collections.Generic;

namespace Quinn.AI.BehaviorTree
{
	public class Tree : IEnumerable<Node>, IEnumerable
	{
		public Enemy Agent { get; }
		public Selector Root { get; }
		public bool DebugMode { get; set; }

		private bool _exited;

		public Tree(Enemy agent)
		{
			Agent = agent;

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
