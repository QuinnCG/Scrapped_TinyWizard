using Quinn.Player;
using UnityEngine;

namespace Quinn.RoomSystem
{
	[RequireComponent(typeof(Collider2D))]
	public class Exit : MonoBehaviour
	{
		[field: SerializeField]
		public ExitDirection Direction { get; private set; } = ExitDirection.Right;

		[field: SerializeField]
		public Room Next { get; private set; }

		public Room Parent { get; set; }
		public bool IgnoreNextTrigger { get; set; }

		private bool _isTriggered;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!_isTriggered
				&& collision.gameObject == PlayerController.Instance.gameObject)
			{
				if (IgnoreNextTrigger)
				{
					IgnoreNextTrigger = false;
				}
				else
				{
					_isTriggered = true;
					RoomManager.Instance.LoadRoom(Next, this);
				}
			}
		}
	}
}
