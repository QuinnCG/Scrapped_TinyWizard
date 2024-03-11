using Sirenix.OdinInspector;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Quinn
{
	[ExecuteAlways]
	public class SaveHandle : MonoBehaviour
	{
		[SerializeField, ReadOnly]
		private string GUID;

		public string ID => GUID;

		private void Update()
		{
#if UNITY_EDITOR
			if (!PrefabStageUtility.GetCurrentPrefabStage())
			{
				if (string.IsNullOrEmpty(GUID))
				{
					GUID = System.Guid.NewGuid().ToString();
				}
			}
#endif
		}
	}
}
