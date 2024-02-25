using System;
using UnityEngine;

namespace Quinn.DamageSystem
{
	[RequireComponent(typeof(Damage))]
	public class Health : MonoBehaviour
	{
		[field: SerializeField]
		public float Max { get; set; } = 100f;

		public float Current { get; private set; }
		public float Percent => Current / Max;
		public bool IsDead { get; private set; }

		public event Action<float> OnHeal, OnDamage;
		public event Action OnDeath;

		private void Awake()
		{
			Current = Max;

			var damage = GetComponent<Damage>();
			damage.OnDamaged += (info, _) => Damage(info.Damage);
		}

		public void Heal(float amount)
		{
			float real = Mathf.Min(amount, Max - Current);
			Current += real;
			OnHeal?.Invoke(real);
		}

		public void Damage(float amount)
		{
			float real = Mathf.Min(amount, Current);
			Current -= real;
			OnDamage?.Invoke(real);

			if (Current == 0f)
			{
				IsDead = true;
				OnDeath?.Invoke();
			}
		}
	}
}
