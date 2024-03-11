using Quinn.Player;
using Quinn.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(SaveHandle))]
	public class ItemPickup : MonoBehaviour, IInteractable
	{
		[SerializeField, Required]
		private Item Item;

		[SerializeField]
		private int Count = 1;

		private string _id;

		private void Awake()
		{
			_id = GetComponent<SaveHandle>().ID;

			if (SaveManager.Has(_id))
			{
				Destroy(gameObject);
			}
		}

		public void OnInteract(PlayerController player)
		{
			Inventory.Instance.Add(Item, Count);

			HUDUI.Instance.DisplayItemPickedup(Item, Count);
			SaveManager.Save(_id);

			Destroy(gameObject);
		}
	}
}
