using Cinemachine;
using Quinn.Player;
using Quinn.SpellSystem;
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

		[SerializeField]
		private float TransitionMoveDistance = 4f;

		public CinemachineVirtualCamera DefaultVirtualCamera { get; private set; }

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

			DefaultVirtualCamera = instance.GetComponentInChildren<CinemachineVirtualCamera>();
		}

		public void LoadRoom(Room next, Exit from)
		{
			StartCoroutine(LoadRoomSequence(next, from));
		}

		private IEnumerator LoadRoomSequence(Room next, Exit from)
		{
			Room nextRoom = SpawnRoom(next, from, out Vector2 dir);

			// Fade to black.
			CameraManager.Instance.FadeToBlack();

			// Animate player moving into next room.
			StartCoroutine(AnimatePlayer(dir));

			float fadeToBlack = CameraManager.Instance.FadeToBlackDuration;
			yield return new WaitForSeconds(fadeToBlack);

			// Unload previous room.
			Destroy(_loadedRoom.gameObject);

			// Fade from black.
			CameraManager.Instance.FadeFromBlack();

			// Set _loadedRoom.
			_loadedRoom = nextRoom;
		}

		private IEnumerator AnimatePlayer(Vector2 dir)
		{
			var player = PlayerController.Instance;
			var input = player.GetComponent<InputReader>();
			var move = player.GetComponent<Movement>();
			move.StopDash();
			player.GetComponent<SpellCaster>().CancelCharge();

			input.enabled = false;

			var dst = float.PositiveInfinity;

			Vector2 start = player.transform.position;
			Vector2 target = start + (dir * TransitionMoveDistance);

			while (dst > 0.2f)
			{
				dst = Vector2.Distance(player.transform.position, target);
				move.Move(dir);

				yield return null;
			}

			input.enabled = true;
		}

		private Room SpawnRoom(Room next, Exit from, out Vector2 dir)
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

			dir = nextRoom.transform.position - nextExit.transform.position;
			dir.Normalize();

			return nextRoom;
		}
	}
}
