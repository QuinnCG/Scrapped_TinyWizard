using Quinn.DamageSystem;
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

		[field: SerializeField, Required]
		public Transform SpellOrigin { get; private set; }

		[field: SerializeField]
		public float BaseDamage { get; set; } = 19f;

		public Damage Damage { get; private set; }
		public bool IsCharging { get; private set; }
		public float Charge { get; private set; }

		public event Action OnBeginCharge;
		public event Action<float> OnReleaseCharge, OnCancelCharge;

		private readonly List<Spell> _spells = new();

		private void Awake()
		{
			Damage = GetComponent<Damage>();
		}

		private void Update()
		{
			if (IsCharging)
			{
				Charge += ChargeRate * Time.deltaTime;
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
				OnBeginCharge?.Invoke();
			}
		}

		public void ReleaseCharge(GameObject spell, Vector2 target)
		{
			if (IsCharging)
			{
				IsCharging = false;

				float max = spell.GetComponent<Spell>().MaxCharge;
				if (Charge > max)
				{
					CastSpell(spell, target);
				}

				Charge = 0f;
				OnReleaseCharge?.Invoke(Charge);
			}
		}

		public void CancelCharge()
		{
			if (IsCharging)
			{
				IsCharging = false;
				Charge = 0f;

				OnCancelCharge?.Invoke(Charge);
			}
		}

		public void CastSpell(GameObject spell, Vector2 target)
		{
			var instance = Instantiate(spell, transform);
			var s = instance.GetComponent<Spell>();
			_spells.Add(s);

			s.Cast(this, Charge, BaseDamage, target);
		}
	}
}
