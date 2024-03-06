using System.Collections;
using System.Collections.Generic;

namespace Quinn.AI.BehaviorTree
{
	public abstract class Composite : Node, IEnumerable<Node>
	{
		protected List<Node> Children { get; } = new();
		protected int Count => Children.Count;

		public void Add(Node node)
		{
			node.SetParent(this);
			Children.Add(node);
		}

		public IEnumerator<Node> GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
