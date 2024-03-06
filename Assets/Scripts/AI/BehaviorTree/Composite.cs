using System.Collections;
using System.Collections.Generic;

namespace Quinn.AI
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

		public override void Enter()
		{
			foreach (var child in Children)
			{
				child.ParentEnter();
			}

			base.Enter();
		}

		public override void Exit()
		{
			foreach (var child in Children)
			{
				child.ParentExit();
			}

			base.Exit();
		}

		public T GetChild<T>(string name = "") where T : Node
		{
			foreach (var child in Children)
			{
				if (child is Composite composite)
				{
					var result = composite.GetChild<T>(name);
					if (result != null) return result;
				}
			}

			return (T)Children.Find(x => x.RawName == name && x.GetType() == typeof(T));
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
