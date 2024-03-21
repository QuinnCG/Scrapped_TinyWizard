using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.Player
{
	public class Crosshair : MonoBehaviour
	{
		public static Crosshair Instance { get; private set; }

		[SerializeField, Required]
		private GameObject Prefab;

		public Vector2 Position { get; private set; }

		private Camera _main;

		private GameObject _crosshair;
		private Material _crosshairMat;

		private void Awake()
		{
			Instance = this;
		}

		private void OnEnable()
		{
			_main = Camera.main;

			_crosshair = Instantiate(Prefab, GetMousePos(), Quaternion.identity);
			_crosshairMat = _crosshair.GetComponent<SpriteRenderer>().material;
		}

		private void Update()
		{
			if (_crosshair != null)
			{
				Vector2 pos = GetMousePos();
				_crosshair.transform.position = pos;

				Position = pos;
			}
		}

		private void OnDisable()
		{
			Destroy(_crosshair);
			_crosshair = null;
		}

		public void SetCharge(float percent)
		{
			if (_crosshairMat)
			{
				_crosshairMat.SetFloat("_Charge", Mathf.Min(percent, 1f));
			}
		}

		public void SetAccuracy(float radius)
		{
			if (_crosshair)
			{
				_crosshair.transform.localScale = Vector3.one * radius;
			}
		}

		private Vector2 GetMousePos()
		{
			return _main.ScreenToWorldPoint(Input.mousePosition);
		}
	}
}
