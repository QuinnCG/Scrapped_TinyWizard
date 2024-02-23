using Quinn.SpellSystem;
using Sirenix.OdinInspector;
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

		[SerializeField, Required]
		private Transform Staff;

		[SerializeField]
		private float StaffOffset = 0.5f;

		[SerializeField, Required]
		private Transform StaffOrigin;

		[SerializeField, FoldoutGroup("StaffColors")]
		private Color Fire, Water, Earth, Lightning, Holy, Nature, Dark;

		[SerializeField, Required]
		private SpriteRenderer StaffRenderer;

		[SerializeField, Required]
		private Transform CameraTarget;

		[SerializeField, Required]
		private float CameraCrosshairBias = 0.2f;

		[SerializeField]
		private float DashCooldown = 0.2f;

		private Animator _animator;
		private Movement _movement;
		private SpellCaster _caster;

		private Spell _equippedSpell;
		private Material _staffMat;

		private float _nextDashTime;

		private void Awake()
		{
			_staffMat = StaffRenderer.material;
			EquipSpell(EquippedSpellPrefab);

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
			CrosshairChargeUpdate();
			StaffTransformUpdate();
			CameraTargetUpdate();
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
			if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextDashTime)
			{
				_nextDashTime = Time.time + _movement.DashDuration + DashCooldown;
				_caster.CancelCharge();

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

		private void CrosshairChargeUpdate()
		{
			if (_equippedSpell)
			{
				Crosshair.Instance.SetAccuracy(_equippedSpell.TargetRadius);
			}

			if (_caster.IsCharging)
			{
				Crosshair.Instance.SetCharge(_caster.Charge / _equippedSpell.MaxCharge);
			}
		}

		private void StaffTransformUpdate()
		{
			Vector2 staffOrigin = StaffOrigin.transform.position;

			Vector2 crosshairPos = Crosshair.Instance.Position;
			Vector2 dirToCrosshair = (crosshairPos - staffOrigin).normalized;

			Staff.position = staffOrigin + (dirToCrosshair * StaffOffset);

			bool toTheLeft = Staff.position.x < transform.position.x;
			Staff.localScale = new Vector3(toTheLeft ? -1f : 1f, 1f, 1f);
		}

		private void CameraTargetUpdate()
		{
			Vector2 a = transform.position;
			Vector2 b = Crosshair.Instance.Position;

			Vector2 target = Vector2.Lerp(a, b, CameraCrosshairBias);
			CameraTarget.position = target;
		}

		private void EquipSpell(GameObject spellPrefab)
		{
			_equippedSpell = spellPrefab.GetComponent<Spell>();
			_staffMat.SetColor("_Color", _equippedSpell.Element switch
			{
				ElementType.Fire => Fire,
				ElementType.Water => Water,
				ElementType.Earth => Earth,
				ElementType.Lightning => Lightning,
				ElementType.Holy => Holy,
				ElementType.Nature => Nature,
				ElementType.Dark => Dark,

				_ => throw new NotImplementedException($"The element {_equippedSpell.Element} is not implemented!")
			});
		}
	}
}
