using System;
using UnityEngine;

namespace Quinn
{
	public class Damage : MonoBehaviour
	{
		[field: SerializeField]
		public Team Team { get; private set; }

		public event Action<DamageInfo> OnDamaged;

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

			OnDamaged?.Invoke(info);
			return true;
		}
	}
}
