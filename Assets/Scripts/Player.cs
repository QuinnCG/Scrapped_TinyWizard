using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Movement))]
	public class Player : MonoBehaviour
	{
		private Animator _animator;
		private Movement _movement;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
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
			_animator.SetBool("IsMoving", _movement.IsMoving);

			if (Input.GetKeyDown(KeyCode.Space))
			{
				_movement.Dash();
				_animator.SetTrigger("Dash");
			}
		}
	}
}
