using DG.Tweening;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Quinn.DamageSystem;
using Quinn.RoomSystem;
using Quinn.SpellSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace Quinn.Player
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(Damage))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(SpellCaster))]
	[RequireComponent(typeof(Collider2D))]
	public class PlayerController : MonoBehaviour
	{
		public static PlayerController Instance { get; private set; }

		[SerializeField]
		private float DashCooldown = 0.2f;

		[SerializeField]
		private float InteractRadius = 2f;

		[field: SerializeField]
		public float MaxMana { get; private set; } = 50f;

		[SerializeField]
		private float ManaRegenRate = 5f;

		[SerializeField, Required, BoxGroup("Spell")]
		private Transform Staff;

		[SerializeField, BoxGroup("Spell")]
		private float StaffOffset = 0.5f;

		[SerializeField, Required, BoxGroup("Spell")]
		private Transform StaffOrigin;

		[SerializeField, Required, FoldoutGroup("Spell/StaffColors")]
		private Light2D StaffLight;

		[SerializeField, FoldoutGroup("Spell/StaffColors")]
		private Color Fire, Water, Earth, Lightning, Holy, Nature, Gravity;

		[SerializeField, FoldoutGroup("Spell/Dash VFX")]
		private VisualEffect FireDash, WaterDash, EarthDash, LightningDash, HolyDash, NatureDash, GravityDash;

		[SerializeField, Required, BoxGroup("Spell")]
		private SpriteRenderer StaffRenderer;

		[SerializeField, Required, BoxGroup("Camera")]
		private Transform CameraTarget;

		[SerializeField, Required, BoxGroup("Camera")]
		private float CameraCrosshairBias = 0.2f;

		[SerializeField, BoxGroup("SFX")]
		private EventReference DashSound;

		[SerializeField, BoxGroup("SFX")]
		private EventReference HurtSnapshot;

		public float Mana { get; private set; }
		public Vector2 Velocity => _movement.Velocity;
		public Vector2 Center => _collider.bounds.center;
		public SpellCaster Caster { get; private set; }

		private Animator _animator;
		private InputReader _input;
		private Movement _movement;
		private Collider2D _collider;
		private Damage _damage;

		private Material _staffMat;

		private float _nextDashTime;
		private GameObject _dashTrail;

		private EventInstance _hurtSnapshot;

		private void Awake()
		{
			Instance = this;

			_staffMat = StaffRenderer.material;

			_animator = GetComponent<Animator>();
			_input = GetComponent<InputReader>();
			_movement = GetComponent<Movement>();
			Caster = GetComponent<SpellCaster>();
			_collider = GetComponent<Collider2D>();
			_damage = GetComponent<Damage>();

			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = false;

			_input.OnDash += OnDash;
			_input.OnCastPress += OnCastPress;
			_input.OnCastRelease += OnCastRelease;
			_input.OnInteract += OnInteract;

			_movement.OnDashEnd += OnDashEnd;

			_damage.OnDamaged += (_, _) =>
			{
				_hurtSnapshot.start();
				DOVirtual.DelayedCall(2f, () => _hurtSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT), false);
			};
			GetComponent<Health>().OnDeath += OnDeath;

			Mana = MaxMana;
			Caster.ManaDelta += OnManaUpdate;
		}

		private void Start()
		{
			Inventory.Instance.OnSpellSelected += UpdateStaffColor;
			UpdateStaffColor(Inventory.Instance.ActiveSpell);

			_hurtSnapshot = RuntimeManager.CreateInstance(HurtSnapshot);
		}

		private void Update()
		{
			MoveUpdate();
			_animator.SetBool("IsMoving", _movement.IsMoving);

			CrosshairChargeUpdate();
			StaffTransformUpdate();
			CameraTargetUpdate();

			float cost = Inventory.Instance.ActiveSpell.ManaCost;
			Caster.CanCharge = Mana >= cost;

			if (!Caster.IsCharging)
			{
				Mana = Mathf.Min(Mana + (ManaRegenRate * Time.deltaTime), MaxMana);
			}
		}

		private void OnDestroy()
		{
			_hurtSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			_hurtSnapshot.release();

			transform.DOKill();
		}

		public static void Respawn(Vector2 position)
		{
			if (Instance != null)
			{
				Destroy(Instance.gameObject);
			}

			Addressables.InstantiateAsync("Player.prefab").Completed += opHandle =>
			{
				GameObject player = opHandle.Result;
				player.transform.position = position;
			};

			Application.Quit(); // TODO: Fix respawning.
		}

		public Transform GetCameraTarget() => CameraTarget;

		private void OnManaUpdate(float delta)
		{
			Mana = Mathf.Clamp(Mana + delta, 0f, MaxMana);
		}

		private void MoveUpdate()
		{
			_movement.Move(_input.MoveInput);

			var dirToCrosshair = Crosshair.Instance.Position - Center;
			_movement.SetFacingDirection(dirToCrosshair.x);
		}

		private void OnDash()
		{
			if (Time.time > _nextDashTime && !Caster.IsCharging)
			{
				_damage.DisableDamage = true;

				_nextDashTime = Time.time + _movement.DashDuration + DashCooldown;

				_movement.Dash();
				_animator.SetTrigger("Dash");

				AudioManager.Play(DashSound, transform);

				var prefab = Inventory.Instance.ActiveSpell.Element switch
				{
					ElementType.Fire => FireDash.gameObject,
					ElementType.Water => WaterDash.gameObject,
					ElementType.Earth => EarthDash.gameObject,
					ElementType.Lightning => LightningDash.gameObject,
					ElementType.Holy => HolyDash.gameObject,
					ElementType.Nature => NatureDash.gameObject,
					ElementType.Gravity => GravityDash.gameObject,
					_ => null
				};

				if (prefab)
				{
					_dashTrail = Instantiate(prefab, transform);
				}
			}
		}

		private void OnCastPress()
		{
			Caster.MaxCharge = Inventory.Instance.ActiveSpell.Prefab.GetComponent<Spell>().MaxCharge;
			Caster.BeginCharge();
		}

		private void OnCastRelease()
		{
			Vector2 target = Crosshair.Instance.Position;
			Caster.ReleaseCharge(Inventory.Instance.ActiveSpell.Prefab, target);
		}

		private void OnInteract()
		{
			var colliders = Physics2D.OverlapCircleAll(transform.position, InteractRadius, GameManager.Instance.InteractableLayer);

			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(typeof(IInteractable), out var component))
				{
					var interactable = component as IInteractable;
					interactable.OnInteract(this);
				}
			}
		}

		private void CrosshairChargeUpdate()
		{
			var spell = Inventory.Instance.ActiveSpell.Prefab.GetComponent<Spell>();

			if (spell)
			{
				Crosshair.Instance.SetAccuracy(spell.TargetRadius);
			}

			Crosshair.Instance.SetCharge(Caster.Charge / spell.MaxCharge);
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

		private void UpdateStaffColor(SpellItem spell)
		{
			var color = spell.Element switch
			{
				ElementType.Fire => Fire,
				ElementType.Water => Water,
				ElementType.Earth => Earth,
				ElementType.Lightning => Lightning,
				ElementType.Holy => Holy,
				ElementType.Nature => Nature,
				ElementType.Gravity => Gravity,

				_ => throw new NotImplementedException($"The element {Inventory.Instance.ActiveSpell.Element} is not implemented!")
			};

			_staffMat.SetColor("_Color", color);
			StaffLight.color = color;
		}

		private void OnDashEnd()
		{
			_dashTrail.transform.parent = null;
			Destroy(_dashTrail, 2f);

			_damage.DisableDamage = false;
		}

		private void OnDeath()
		{
			RoomManager.Instance.ReloadRoom();
		}
	}
}
