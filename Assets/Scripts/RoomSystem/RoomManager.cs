using Cinemachine;
using FMODUnity;
using Quinn.Player;
using Quinn.SpellSystem;
using Sirenix.OdinInspector;
using System;
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

		public Room CurrentRoom { get; private set; }
		public CinemachineVirtualCamera DefaultVirtualCamera { get; private set; }
		public RegionType CurrentRegion { get; private set; }

		public event Action<RegionType> OnRegionChange;

		private float _playerTransitionProgress;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			if (CurrentRoom == null)
			{
				LoadDefaultRoom();
			}
		}

		public void LoadDefaultRoom(GameObject room = null)
		{
			GameObject prefab = DefaultRoom.gameObject;

			if (room != null)
			{
				prefab = room;
			}

			var instance = Instantiate(prefab, transform);
			CurrentRoom = instance.GetComponent<Room>();

			ChangeRegion(CurrentRoom);

			DefaultVirtualCamera = instance.GetComponentInChildren<CinemachineVirtualCamera>();
			CameraManager.Instance.SetVirtualCamera(DefaultVirtualCamera);
		}

		public void LoadRoom(Room next, Exit from)
		{
			StartCoroutine(LoadRoomSequence(next, from));
			ChangeRegion(next);
		}

		private IEnumerator LoadRoomSequence(Room next, Exit from)
		{
			Room nextRoom = SpawnRoom(next, from, out Vector2 dir);

			// Fade to black.
			CameraManager.Instance.FadeToBlack();

			// Animate player moving into next room.
			_playerTransitionProgress = 0f;
			StartCoroutine(AnimatePlayer(dir, from.transform.position));

			float fadeToBlack = CameraManager.Instance.FadeToBlackDuration;
			yield return new WaitForSeconds(fadeToBlack);

			// Unload previous room.
			Destroy(CurrentRoom.gameObject);

			yield return new WaitUntil(() => _playerTransitionProgress > 0.5f);

			// Fade from black.
			CameraManager.Instance.FadeFromBlack();

			// Set _loadedRoom.
			CurrentRoom = nextRoom;
		}

		private IEnumerator AnimatePlayer(Vector2 dir, Vector2 exitOrigin)
		{
			var player = PlayerController.Instance;
			var input = player.GetComponent<InputReader>();
			var move = player.GetComponent<Movement>();
			move.StopDash();
			player.GetComponent<SpellCaster>().CancelCharge();

			var inputDisabler = input.DisableInput();
			var dst = float.PositiveInfinity;

			Vector2 start = exitOrigin;
			Vector2 target = start + (dir * TransitionMoveDistance);

			float maxDst = Vector2.Distance(start, target);

			const float StoppingDst = 1.5f;
			while (dst > StoppingDst)
			{
				dst = Vector2.Distance(player.transform.position, target);
				move.Move(dir);

				_playerTransitionProgress = dst / maxDst;

				yield return null;
			}

			input.EnableInput(inputDisabler);
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

		private void ChangeRegion(Room room)
		{
			if (CurrentRegion != room.Region)
			{
				CurrentRegion = room.Region;

				//RuntimeManager.StudioSystem.setParameterByName("region", (float)region);
				OnRegionChange?.Invoke(CurrentRegion);
			}
		}
	}
}
