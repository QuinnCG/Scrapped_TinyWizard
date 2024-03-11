using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn.Player
{
	public class Inventory : MonoBehaviour
	{
		public static Inventory Instance { get; private set; }

		[SerializeField]
		private SpellItem[] DefaultSpells;

		public IEnumerable<Item> Items => _inventory.Keys.AsEnumerable();
		public IEnumerable<SpellItem> EquippedSpells => _equippedSpells.AsEnumerable();
		public SpellItem ActiveSpell { get; private set; }

		public event Action<SpellItem> OnSpellSelected;

		private readonly Dictionary<Item, int> _inventory = new();
		private readonly List<SpellItem> _equippedSpells = new();

		private void Awake()
		{
			Instance = this;

			foreach (var spell in DefaultSpells)
			{
				EquipSpell(spell);
			}

			SelectSpell(0);
		}

		public void EquipSpell(SpellItem spell)
		{
			if (!_equippedSpells.Contains(spell))
			{
				_equippedSpells.Add(spell);
			}
		}

		public void DequipSpell(SpellItem spell)
		{
			_equippedSpells.Remove(spell);
		}

		public void SelectSpell(int index)
		{
			ActiveSpell = _equippedSpells[index];
			OnSpellSelected?.Invoke(ActiveSpell);
		}

		public bool ContainsItem(Item item)
		{
			return _inventory.ContainsKey(item);
		}

		public void Add(Item item, int amount = 1)
		{
			if (_inventory.ContainsKey(item))
			{
				_inventory[item] += amount;
				return;
			}

			_inventory.Add(item, amount);

			// This is a temporary fix because there is no proper inventory UI.
			if (item is SpellItem spell)
			{
				EquipSpell(spell);
			}
		}

		public void Remove(Item item, int amount)
		{
			if (_inventory.ContainsKey(item))
			{
				_inventory[item] -= amount;

				if (_inventory[item] <= 0)
				{
					_inventory.Remove(item);
				}
			}
		}

		public int GetItemCount(Item item)
		{
			if (_inventory.ContainsKey(item))
			{
				return _inventory[item];
			}

			return 0;
		}
	}
}
