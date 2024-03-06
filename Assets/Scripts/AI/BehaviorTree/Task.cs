using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Quinn.AI.BehaviorTree
{
	public abstract class Task : Node
	{
		public override void Enter()
		{
			base.Enter();

#if UNITY_EDITOR
			if (Tree.DebugMode)
			{
				var branch = new List<Node>() { this };

				Node parent = Parent;
				while (parent != null)
				{
					branch.Insert(0, parent);
					parent = parent.Parent;
				}

				var builder = new StringBuilder();
				builder.AppendLine($"Current Task: <color=yellow><b>{Name}</b></color>");

				for (int i = 0; i < branch.Count; i++)
				{
					builder.Append($"<color=yellow>{branch[i].Name}</color>");

					if (i < branch.Count - 1)
					{
						builder.Append(" -> ");
					}
				}

				Debug.Log(builder, Agent);
			}
#endif
		}
	}
}
