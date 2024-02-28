using Quinn.SpellSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn.Player
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(SpellCaster))]
	[RequireComponent(typeof(Collider2D))]
	public class PlayerController : MonoBehaviour
	{
		public static PlayerController Instance { get; private set; }

		[SerializeField, BoxGroup("Movement")]
		private float DashCooldown = 0.2f;

		[field: SerializeField, BoxGroup("Spell")]
		public GameObject EquippedSpellPrefab { get; private set; }

		[SerializeField, Required, BoxGroup("Spell")]
		private Transform Staff;

		[SerializeField, BoxGroup("Spell")]
		private float StaffOffset = 0.5f;

		[SerializeField, Required, BoxGroup("Spell")]
		private Transform StaffOrigin;

		[SerializeField, FoldoutGroup("Spell/StaffColors")]
		private Color Fire, Water, Earth, Lightning, Holy, Nature, Dark;

		[SerializeField, Required, BoxGroup("Spell")]
		private SpriteRenderer StaffRenderer;

		[SerializeField, Required, BoxGroup("Camera")]
		private Transform CameraTarget;

		[SerializeField, Required, BoxGroup("Camera")]
		private float CameraCrosshairBias = 0.2f;

		public Vector2 Velocity => _movement.Velocity;
		public Vector2 Center => _collider.bounds.center;

		private Animator _animator;
		private InputReader _input;
		private Movement _movement;
		private SpellCaster _caster;
		private Collider2D _collider;

		private Spell _equippedSpell;
		private Material _staffMat;

		private float _nextDashTime;

		private void Awake()
		{
			Instance = this;

			_staffMat = StaffRenderer.material;
			EquipSpell(EquippedSpellPrefab);

			_animator = GetComponent<Animator>();
			_input = GetComponent<InputReader>();
			_movement = GetComponent<Movement>();
			_caster = GetComponent<SpellCaster>();
			_collider = GetComponent<Collider2D>();

			_caster.OnReleaseCharge += OnStopCharge;
			_caster.OnCancelCharge += OnStopCharge;

			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = false;

			_input.OnDash += OnDash;
			_input.OnCastPress += OnCastPress;
			_input.OnCastRelease += OnCastRelease;
		}

		private void Update()
		{
			MoveUpdate();
			_animator.SetBool("IsMoving", _movement.IsMoving);

			CrosshairChargeUpdate();
			StaffTransformUpdate();
			CameraTargetUpdate();
		}

		private void MoveUpdate()
		{
			_movement.Move(_input.MoveInput);

			var dirToCrosshair = Crosshair.Instance.Position - Center;
			_movement.SetFacingDirection(dirToCrosshair.normalized.x);
		}

		private void OnDash()
		{
			if (Time.time > _nextDashTime && !_caster.IsCharging)
			{
				_nextDashTime = Time.time + _movement.DashDuration + DashCooldown;

				_movement.Dash();
				_animator.SetTrigger("Dash");
			}
		}

		private void OnCastPress()
		{
			_caster.BeginCharge();
		}

		private void OnCastRelease()
		{
			Vector2 target = Crosshair.Instance.Position;
			_caster.ReleaseCharge(EquippedSpellPrefab, target);
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
