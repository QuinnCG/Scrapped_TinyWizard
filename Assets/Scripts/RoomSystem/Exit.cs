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

		private void OnDrawGizmos()
		{
			Vector3 dir = Direction switch
			{
				ExitDirection.Up => Vector2.up,
				ExitDirection.Down => Vector2.down,
				ExitDirection.Left => Vector2.left,
				ExitDirection.Right => Vector2.right,
				_ => throw new System.Exception("Invalid exit direction!")
			};

			Gizmos.DrawLine(transform.position, transform.position + (dir * 4f));
		}
	}
}
