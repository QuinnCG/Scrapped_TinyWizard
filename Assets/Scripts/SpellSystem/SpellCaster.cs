using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.SpellSystem
{
	public class SpellCaster : MonoBehaviour
	{
		[SerializeField]
		private float ChargeRate = 50f;

		public bool IsCharging { get; private set; }
		public float Charge { get; private set; }

		public event Action OnBeginCharge;
		public event Action<float> OnReleaseCharge, OnCancelCharge;

		private readonly List<Spell> _spells = new();

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
			}
		}

		public void ReleaseCharge(GameObject spell)
		{
			if (IsCharging)
			{
				OnReleaseCharge?.Invoke(Charge);

				float max = spell.GetComponent<Spell>().MaxCharge;
				if (Charge > max)
				{
					CastSpell(spell);
				}

				Charge = 0f;
			}
		}

		public void CancelCharge()
		{
			if (IsCharging)
			{
				OnCancelCharge?.Invoke(Charge);
				Charge = 0f;
			}
		}

		public void CastSpell(GameObject spell)
		{
			var instance = Instantiate(spell, transform);
			var s = instance.GetComponent<Spell>();
			_spells.Add(s);

			s.Cast(this, Charge);
		}
	}
}
