using Cinemachine;
using Quinn.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.RoomSystem
{
	public class CameraRegion : MonoBehaviour
	{
		[SerializeField, Required]
		private CinemachineVirtualCamera VirtualCamera;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject == PlayerController.Instance.gameObject)
			{
				CameraManager.Instance.SetVirtualCamera(VirtualCamera);
			}
		}
	}
}
