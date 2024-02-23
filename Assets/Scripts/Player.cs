using Quinn.SpellSystem;
using System;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(SpellCaster))]
	public class Player : MonoBehaviour
	{
		[field: SerializeField]
		public GameObject EquippedSpellPrefab { get; private set; }

		private Animator _animator;
		private Movement _movement;
		private SpellCaster _caster;

		private Spell _equippedSpell;

		private void Awake()
		{
			_equippedSpell = EquippedSpellPrefab.GetComponent<Spell>();

			_animator = GetComponent<Animator>();
			_movement = GetComponent<Movement>();
			_caster = GetComponent<SpellCaster>();

			_caster.OnReleaseCharge += OnStopCharge;
			_caster.OnCancelCharge += OnStopCharge;

			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = false;
		}

		private void Update()
		{
			MoveUpdate();
			DashUpdate();
			CastUpdate();

			if (_equippedSpell)
			{
				Crosshair.Instance.SetAccuracy(_equippedSpell.TargetRadius);
			}

			if (_caster.IsCharging)
			{
				Crosshair.Instance.SetCharge(_caster.Charge / _equippedSpell.MaxCharge);
			}
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

			if (Input.GetMouseButtonUp(0))
			{
				Vector2 target = Crosshair.Instance.Position;
				_caster.ReleaseCharge(EquippedSpellPrefab, target);
			}
		}

		private void OnStopCharge(float charge)
		{
			Crosshair.Instance.SetCharge(0f);
		}
	}
}
