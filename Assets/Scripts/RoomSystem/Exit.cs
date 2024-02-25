using Quinn.Player;
using UnityEngine;

namespace Quinn.RoomSystem
{
	[RequireComponent(typeof(Collider2D))]
	public class Exit : MonoBehaviour
	{
		[field: SerializeField]
		public ExitDirection Direction { get; private set; }

		[field: SerializeField]
		public Room Next { get; private set; }

		public Room Parent { get; set; }

		private bool _isTriggered;

		private void OnTriggerStay2D(Collider2D collision)
		{
			if (_isTriggered && collision.gameObject == PlayerController.Instance.gameObject)
			{
				_isTriggered = true;
				RoomManager.Instance.LoadRoom(Next, this);
			}
		}
	}
}
