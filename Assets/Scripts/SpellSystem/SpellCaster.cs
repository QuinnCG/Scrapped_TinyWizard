using System.Collections.Generic;
using UnityEngine;

namespace Quinn.SpellSystem
{
	public class SpellCaster : MonoBehaviour
	{
		private readonly List<Spell> _spells = new();

		private void OnDestroy()
		{
			foreach (var spell in _spells)
			{
				spell.OnCasterDeath();
			}
		}

		public void CastSpell(GameObject prefab)
		{
			var instance = Instantiate(prefab, transform);
			var spell = instance.GetComponent<Spell>();
			_spells.Add(spell);

			spell.Cast(this);
		}
	}
}
