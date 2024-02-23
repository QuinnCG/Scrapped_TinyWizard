using UnityEngine;

namespace Quinn.Player
{
	[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item", order = 0)]
	public class Item : ScriptableObject
	{
		public string Name = "Item Name";

		[Multiline]
		public string Description = "This is the item's description.";
	}
}
