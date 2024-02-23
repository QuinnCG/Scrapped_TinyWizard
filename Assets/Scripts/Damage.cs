using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn
{
	public class Damage : MonoBehaviour
	{
		[field: SerializeField]
		public Team Team { get; private set; }

		[SerializeField, BoxGroup("Elemental")]
		private ElementType Resistances;

		[SerializeField, BoxGroup("Elemental")]
		private ElementType Weaknesses;

		[Space, SerializeField, BoxGroup("Elemental")]
		private float ResistanceDamageFactor = 0.75f;

		[SerializeField, BoxGroup("Elemental")]
		private float WeaknessDamageFactor = 1.5f;

		public event Action<DamageInfo, DamageEfficiencyType> OnDamaged;

		public bool CanTakeDamage(Team source)
		{
			return Team != source;
		}

		public bool TakeDamage(DamageInfo info)
		{
			if (!CanTakeDamage(info.Source.Team))
			{
				return false;
			}

			var efficiencyType = ModifyDamage(info);
			OnDamaged?.Invoke(info, efficiencyType);

			return true;
		}

		private DamageEfficiencyType ModifyDamage(DamageInfo info)
		{
			float modifier = 1f;
			var efficiencyType = DamageEfficiencyType.Normal;

			if ((info.Element | Resistances) > 0)
			{
				modifier += ResistanceDamageFactor;
				efficiencyType = DamageEfficiencyType.Resistant;
			}
			else if ((info.Element | Weaknesses) > 0)
			{
				modifier += WeaknessDamageFactor;
				efficiencyType = DamageEfficiencyType.Weak;
			}

			info.Damage *= modifier;
			return efficiencyType;
		}
	}
}
