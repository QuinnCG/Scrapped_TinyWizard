using Unity.Services.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn
{
	public class GameManager : MonoBehaviour
	{
		public const string ObstacleLayerName = "Obstacle";
		public const string CharacterLayerName = "Character";

		public static GameManager Instance { get; private set; }

		[field: SerializeField]
		public LayerMask ObstacleLayer { get; private set; }

		[field: SerializeField]
		public LayerMask CharacterLayer { get; private set; }

		public bool IsEasyMode { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var instance = Addressables.InstantiateAsync("GameManager.prefab")
				.WaitForCompletion();

			DontDestroyOnLoad(instance);
		}

		private async void Awake()
		{
			Instance = this;
			await UnityServices.InitializeAsync();
		}

		public void SetEasyMode(bool enabled)
		{
			IsEasyMode = enabled;
		}
	}
}
