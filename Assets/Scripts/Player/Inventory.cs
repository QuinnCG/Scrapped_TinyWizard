using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn.Player
{
	public class Inventory : MonoBehaviour
	{
		public IEnumerable<Item> Items => _inventory.Keys.AsEnumerable();

		private readonly Dictionary<Item, int> _inventory = new();

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
