using Quinn.SpellSystem;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(SpellCaster))]
	public class Player : MonoBehaviour
	{
		[field: SerializeField]
		public GameObject EquippedSpell { get; private set; }

		private Animator _animator;
		private Movement _movement;
		private SpellCaster _caster;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_movement = GetComponent<Movement>();
			_caster = GetComponent<SpellCaster>();

			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = false;
		}

		private void Update()
		{
			MoveUpdate();
			DashUpdate();
			CastUpdate();
		}

		private void MoveUpdate()
		{
			var moveInput = new Vector2()
			{
				x = Input.GetAxisRaw("Horizontal"),
				y = Input.GetAxisRaw("Vertical")
			}.normalized;

			_movement.Move(moveInput);
			_animator.SetBool("IsMoving", _movement.IsMoving);
		}

		private void DashUpdate()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				_movement.Dash();
				_animator.SetTrigger("Dash");
			}
		}

		private void CastUpdate()
		{
			if (Input.GetMouseButtonDown(0))
			{
				_caster.BeginCharge();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				Vector2 target = Crosshair.Instance.Position;
				_caster.ReleaseCharge(EquippedSpell, target);
			}
		}
	}
}
