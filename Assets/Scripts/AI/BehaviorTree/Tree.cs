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

		public Tree(Enemy agent)
		{
			Agent = agent;

			Root = new Selector();
			Root.SetTree(this);
		}

		public void Start()
		{
			Root.Enter();
		}

		public void Update()
		{
			var status = Root.Update();

			if (status != Status.Running)
			{
				Root.Exit();
				Root.Enter();
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
