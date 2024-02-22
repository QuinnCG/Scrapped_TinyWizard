using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Movement))]
	public class Player : MonoBehaviour
	{
		private Movement _movement;

		private void Awake()
		{
			_movement = GetComponent<Movement>();
		}

		private void Update()
		{
			var moveInput = new Vector2()
			{
				x = Input.GetAxisRaw("Horizontal"),
				y = Input.GetAxisRaw("Vertical")
			}.normalized;

			_movement.Move(moveInput);

			if (Input.GetKeyDown(KeyCode.Space))
			{
				_movement.Dash();
			}
		}
	}
}
