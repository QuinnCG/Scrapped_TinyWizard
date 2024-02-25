using UnityEngine;

namespace Quinn.RoomSystem
{
	public class Room : MonoBehaviour
	{
		[field: SerializeField]
		public Exit[] Exits { get; private set; }

		private void Start()
		{
			foreach (var exit in Exits)
			{
				Debug.Assert(exit, $"Room: {gameObject.name} is missing a reference to an exit!");
				exit.Parent = this;
			}
		}
	}
}
