using Quinn.Player;
using Quinn.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Collider2D))]
	public class ItemPickup : MonoBehaviour, IInteractable
	{
		[SerializeField, Required]
		private Item Item;

		[SerializeField]
		private int Count = 1;

		public void OnInteract(PlayerController player)
		{
			HUDUI.Instance.DisplayItemPickedup(Item, Count);
			Destroy(gameObject);
		}
	}
}
