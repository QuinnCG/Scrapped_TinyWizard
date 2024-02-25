using System.Collections;
using UnityEngine;

namespace Quinn.RoomSystem
{
	public class RoomManager : MonoBehaviour
	{
		public static RoomManager Instance { get; private set; }

		private Room _loadedRoom;

		private void Awake()
		{
			Instance = this;
		}

		public void LoadRoom(Room next, Exit from)
		{
			StartCoroutine(LoadRoomSequence(next, from));
		}

		private IEnumerator LoadRoomSequence(Room next, Exit from)
		{
			GameObject nextInstance = Instantiate(next.gameObject, transform);
			Room nextRoom = next.GetComponent<Room>();

			ExitDirection to = (ExitDirection)((int)from.Direction * -1);
			Exit toExit = null;

			foreach (var exit in nextRoom.Exits)
			{
				if (exit.Direction == to)
				{
					toExit = exit;
				}
			}

			Debug.Assert(toExit, "Can't find valid exit!");

			Vector2 offset = from.transform.position - from.Parent.transform.position;
			offset += (Vector2)(nextRoom.transform.position - toExit.transform.position);
			nextInstance.transform.position = offset;

			yield break;
		}
	}
}
