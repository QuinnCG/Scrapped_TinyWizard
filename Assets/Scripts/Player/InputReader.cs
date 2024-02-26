using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.Player
{
	public class InputReader : MonoBehaviour
	{
		public static InputReader Instance { get; private set; }

		public class InputDisablerHandle { }

		public Vector2 MoveInput { get; private set; }

		public event Action OnDash;
		public event Action OnCastPress, OnCastRelease;

		private readonly HashSet<InputDisablerHandle> _disablerHandles = new();

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			if (_disablerHandles.Count > 0) return;

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

			if (Input.GetMouseButtonDown(0))
			{
				OnCastPress?.Invoke();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				OnCastRelease?.Invoke();
			}
		}

		private void OnDisable()
		{
			MoveInput = Vector2.zero;
		}

		public InputDisablerHandle DisableInput()
		{
			var handle = new InputDisablerHandle();
			_disablerHandles.Add(handle);
			return handle;
		}

		public void EnableInput(InputDisablerHandle handle)
		{
			_disablerHandles.Remove(handle);
		}
	}
}
