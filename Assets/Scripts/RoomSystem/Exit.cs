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

		[SerializeField]
		private bool StartArmed;

		public Room Parent { get; set; }

		private bool _isTriggered;
		private bool _canTrigger;

		private void Awake()
		{
			_canTrigger = StartArmed;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!_isTriggered && collision.gameObject == PlayerController.Instance.gameObject)
			{
				if (_canTrigger)
				{
					_isTriggered = true;
					RoomManager.Instance.LoadRoom(Next, this);
				}
				else
				{
					_canTrigger = true;
				}
			}
		}
	}
}
