using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.Player
{
	public class InputReader : MonoBehaviour
	{
		public static InputReader Instance { get; private set; }

		public class InputDisablerHandle
		{
			public bool AllowMovement = false;

			public InputDisablerHandle() { }
			public InputDisablerHandle(bool allowMovement)
			{
				AllowMovement = allowMovement;
			}
		}

		public Vector2 MoveInput { get; private set; }
		public bool IsInputEnabled => _disablerHandles.Count == 0;

		public event Action OnDash;
		public event Action OnCastPress, OnCastRelease;

		private readonly HashSet<InputDisablerHandle> _disablerHandles = new();

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			MoveInput = Vector2.zero;

			bool inputEnabled = _disablerHandles.Count == 0;
			bool allowMovement = true;

			foreach (var handle in _disablerHandles)
			{
				if (!handle.AllowMovement)
				{
					allowMovement = false;
					break;
				}
			}

			if (inputEnabled || allowMovement)
			{
				MoveInput = new Vector2()
				{
					x = Input.GetAxisRaw("Horizontal"),
					y = Input.GetAxisRaw("Vertical")
				}.normalized;

				if (Input.GetKeyDown(KeyCode.Space)
					|| Input.GetKeyDown(KeyCode.LeftShift)
					|| Input.GetKeyDown(KeyCode.LeftControl)
					|| Input.GetMouseButtonDown(1))
				{
					OnDash?.Invoke();
				}
			}

			if (inputEnabled)
			{
				if (Input.GetMouseButtonDown(0))
				{
					OnCastPress?.Invoke();
				}
				else if (Input.GetMouseButtonUp(0))
				{
					OnCastRelease?.Invoke();
				}
			}
		}

		private void OnDisable()
		{
			MoveInput = Vector2.zero;
		}

		public InputDisablerHandle DisableInput(bool allowMovement = false)
		{
			var handle = new InputDisablerHandle()
			{
				AllowMovement = allowMovement
			};

			_disablerHandles.Add(handle);
			return handle;
		}

		public void EnableInput(InputDisablerHandle handle)
		{
			_disablerHandles.Remove(handle);
		}
	}
}
