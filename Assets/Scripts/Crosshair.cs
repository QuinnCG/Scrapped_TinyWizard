using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class Crosshair : MonoBehaviour
	{
		public static Crosshair Instance { get; private set; }

		[SerializeField, Required]
		private GameObject Prefab;

		public Vector2 Position
		{
			get
			{
				return _crosshair ? _crosshair.transform.position : Vector2.zero;
			}
		}

		private Camera _main;
		private GameObject _crosshair;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			_main = Camera.main;
		}

		private void OnEnable()
		{
			_crosshair = Instantiate(Prefab);
		}

		private void OnDisable()
		{
			Destroy(_crosshair);
		}

		private void Update()
		{
			if (_crosshair != null)
			{
				Vector2 pos = _main.ScreenToWorldPoint(Input.mousePosition);
				_crosshair.transform.position = pos;
			}
		}
	}
}
