using UnityEngine;

namespace Quinn.RoomSystem
{
	public class Room : MonoBehaviour
	{
		[field: SerializeField]
		public Exit[] Exits { get; private set; }

		private void Start()
		{
			foreach (var exit in Exits)
			{
				exit.Parent = this;
			}
		}
	}
}
