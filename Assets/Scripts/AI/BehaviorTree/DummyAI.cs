using UnityEngine;

namespace Quinn.AI.BehaviorTree
{
	public class DummyAI : MonoBehaviour
	{
		[SerializeField]
		private bool DebugMode;

		private Tree _tree;

		private void Start()
		{
			_tree = new(null)
			{
				new Composites.Sequence(new Conditionals.Custom(() => Input.GetKey(KeyCode.G)))
				{
					new Tasks.Log("Space is down!"),
					new Tasks.Wait(2f)
				},
				new Tasks.Log("Space Up")
				{
					Name = "Space is up"
				}
			};

			_tree.DebugMode = DebugMode;
			_tree.Start();
		}

		private void Update()
		{
			_tree.Update();
		}
	}
}
