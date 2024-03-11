using Quinn.DamageSystem;
using Quinn.Player;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.SpellSystem
{
	[RequireComponent(typeof(Damage))]
	public class SpellCaster : MonoBehaviour
	{
		[SerializeField]
		private float ChargeRate = 50f;

		[SerializeField]
		private float DecayRate = 150f;

		[field: SerializeField, Required]
		public Transform SpellOrigin { get; private set; }

		[field: SerializeField]
		public float BaseDamage { get; set; } = 19f;

		[SerializeField]
		private float CasterKnockbackScale = 1f;

		[SerializeField, FoldoutGroup("Cast VFX")]
		private GameObject FireCastVFX;

		[SerializeField, FoldoutGroup("Cast VFX")]
		private GameObject WaterCastVFX;

		[SerializeField, FoldoutGroup("Cast VFX")]
		private GameObject EarthCastVFX;

		[SerializeField, FoldoutGroup("Cast VFX")]
		private GameObject LightningCastVFX;

		[SerializeField, FoldoutGroup("Cast VFX")]
		private GameObject HolyCastVFX;

		[SerializeField, FoldoutGroup("Cast VFX")]
		private GameObject NatureCastVFX;

		[SerializeField, FoldoutGroup("Cast VFX")]
		private GameObject DarkCastVFX;

		public Damage Damage { get; private set; }
		public bool IsCharging { get; private set; }
		public float Charge { get; private set; }
		public bool DidJustCastSpell { get; private set; }

		public bool WillSpellFail { get; set; }
		public float MaxCharge { get; set; } = -1f;

		public event Action OnBeginCharge;
		public event Action<float> OnReleaseCharge, OnCancelCharge;
		public event Action<float> ChargeDelta;
		public event Action<float> ManaDelta;

		public event Action<Spell> OnSpellSpawned;

		private Knockback _knockback;
		private readonly List<Spell> _spells = new();
		private bool _waitedOneFrame;

		private float _lastMana;

		private void Awake()
		{
			Damage = GetComponent<Damage>();
			TryGetComponent(out _knockback);
		}

		private void Update()
		{
			if (IsCharging)
			{
				float delta = ChargeRate * Time.deltaTime;
				Charge += delta;
				ChargeDelta?.Invoke(delta);

				if (MaxCharge > -1f)
				{
					Charge = Mathf.Min(Charge, MaxCharge + 1f);
				}
			}
			else
			{
				float delta = Charge > 0f ? DecayRate * Time.deltaTime : 0f;
				Charge -= delta;
				Charge = Mathf.Max(0f, Charge);

				ChargeDelta?.Invoke(-delta);
			}

			if (MaxCharge > -1f && Charge > 0f && Charge < MaxCharge)
			{
				float spellCost = Inventory.Instance.ActiveSpell.ManaCost;
				float mana = Mathf.Lerp(0f, spellCost, Charge / MaxCharge);

				ManaDelta?.Invoke(-(mana - _lastMana));
				_lastMana = mana;
			}

			if (_waitedOneFrame)
			{
				DidJustCastSpell = false;
				_waitedOneFrame = false;
			}
			else
			{
				_waitedOneFrame = true;
			}
		}

		private void OnDestroy()
		{
			foreach (var spell in _spells)
			{
				spell.OnCasterDeath();
			}
		}

		public void BeginCharge()
		{
			if (!IsCharging)
			{
				IsCharging = true;
				Charge = 0f;
				_lastMana = 0f;
				OnBeginCharge?.Invoke();
			}
		}

		public void ReleaseCharge(GameObject spell, Vector2 target)
		{
			if (IsCharging)
			{
				IsCharging = false;

				float max = spell.GetComponent<Spell>().MaxCharge;
				if (Charge > max && !WillSpellFail)
				{
					CastSpell(spell, target);
					Charge = 0f;
				}

				OnReleaseCharge?.Invoke(Charge);
			}
		}

		public void CancelCharge()
		{
			if (IsCharging)
			{
				IsCharging = false;
				Charge = 0f;
				_lastMana = 0f;

				OnCancelCharge?.Invoke(Charge);
			}
		}

		public void CastSpell(GameObject spellPrefab, Vector2 target)
		{
			var instance = Instantiate(spellPrefab, transform);
			var spell = instance.GetComponent<Spell>();
			_spells.Add(spell);

			SpawnCastVFX(spell.Element);
			spell.Cast(this, Charge, BaseDamage, target);

			if (_knockback)
			{
				_knockback.ApplyKnockback((Vector2)SpellOrigin.position - target, spell.CasterKnockbackSpeed * CasterKnockbackScale);
			}

			spell.OnDestroyed += () =>
			{
				_spells.Remove(spell);
			};

			DidJustCastSpell = true;
			OnSpellSpawned?.Invoke(spell);
		}

		private void SpawnCastVFX(ElementType element)
		{
			GameObject prefab = element switch
			{
				ElementType.Fire => FireCastVFX,
				ElementType.Water => WaterCastVFX,
				ElementType.Earth => EarthCastVFX,
				ElementType.Lightning => LightningCastVFX,
				ElementType.Holy => HolyCastVFX,
				ElementType.Nature => NatureCastVFX,
				ElementType.Gravity => DarkCastVFX,
				_ => null
			};

			if (prefab)
			{
				var instance = Instantiate(prefab, SpellOrigin);
				Destroy(instance, 2f);
			}
		}
	}
}
