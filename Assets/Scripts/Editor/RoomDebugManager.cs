using Quinn.Player;
using Quinn.RoomSystem;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Quinn.Editor
{
	[ExecuteInEditMode]
	public class RoomDebugManager : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnPlayModeEnter()
		{
			var stage = PrefabStageUtility.GetCurrentPrefabStage();

			if (stage != null)
			{
				GameObject prefab = stage.prefabContentsRoot;

				if (prefab.TryGetComponent(out Room room))
				{
					RoomManager.Instance.LoadDefaultRoom(prefab);
					PlayerController.Instance.transform.position = room.SpawnPosition;
				}
			}
		}
	}
}
