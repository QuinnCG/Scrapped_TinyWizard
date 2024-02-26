using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Quinn.RoomSystem
{
	public class RoomManager : MonoBehaviour
	{
		public static RoomManager Instance { get; private set; }

		[SerializeField, Tooltip("The gap between room connections.")]
		private float RoomGap = 1f;

		[SerializeField, Required]
		private Room DefaultRoom;

		private Room _loadedRoom;

		private void Awake()
		{
			Instance = this;
			LoadDefaultRoom();
		}

		public void LoadDefaultRoom()
		{
			var instance = Instantiate(DefaultRoom.gameObject, transform);
			_loadedRoom = instance.GetComponent<Room>();
		}

		public void LoadRoom(Room next, Exit from)
		{
			StartCoroutine(LoadRoomSequence(next, from));
		}

		private IEnumerator LoadRoomSequence(Room next, Exit from)
		{
			Room nextRoom = SpawnRoom(next, from);

			// Unload previous room.
			Destroy(_loadedRoom.gameObject);

			// Set _loadedRoom.
			_loadedRoom = nextRoom;

			yield break;
		}

		private Room SpawnRoom(Room next, Exit from)
		{
			// Spawn next room.
			GameObject nextInstance = Instantiate(next.gameObject, transform);
			Room nextRoom = nextInstance.GetComponent<Room>();

			// Get inverted direction.
			ExitDirection to = (ExitDirection)((int)from.Direction * -1);
			Exit nextExit = null;

			// Find corrosponding exit.
			foreach (var exit in nextRoom.Exits)
			{
				if (exit.Direction == to)
				{
					nextExit = exit;
				}
			}

			Debug.Assert(nextExit, "Can't find valid exit!");

			// Position next room.
			Vector2 origin = from.transform.position;
			Vector2 offset = next.transform.position - nextExit.transform.position;
			Vector2 position = origin + offset + (offset.normalized * RoomGap);

			nextInstance.transform.position = position;

			// Prime exit of next room.
			nextExit.IgnoreNextTrigger = true;

			return nextRoom;
		}
	}
}
