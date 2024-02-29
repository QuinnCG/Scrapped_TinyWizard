using UnityEngine;

namespace Quinn.RoomSystem
{
	public class Room : MonoBehaviour
	{
		[field: SerializeField]
		public Exit[] Exits { get; private set; }

		[SerializeField]
		private Transform DebugSpawnPoint;

		public Vector2 DebugSpawnPosition
		{
			get
			{
				if (DebugSpawnPoint != null)
				{
					return DebugSpawnPoint.position;
				}

				if (Exits.Length > 0)
				{
					Exits[0].IgnoreNextTrigger = true;
					return Exits[0].transform.position;
				}

				return transform.position;
			}
		}

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
