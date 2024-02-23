using Quinn.SpellSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.Player
{
	[CreateAssetMenu(fileName = "Spell", menuName = "Inventory/Spell", order = 1)]
	public class SpellItem : Item
	{
		[Required]
		public GameObject SpellPrefab;

		[HideInInspector]
		public ElementType Element;

		private void OnEnable()
		{
			if (SpellPrefab != null)
			{
				Element = SpellPrefab.GetComponent<Spell>().Element;
			}
		}
	}
}
