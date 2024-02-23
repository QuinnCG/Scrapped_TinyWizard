using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.Player
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
		private Material _crosshairMat;

		private void Awake()
		{
			Instance = this;
		}

		private void OnEnable()
		{
			_crosshair = Instantiate(Prefab); ;
			_crosshairMat = _crosshair.GetComponent<SpriteRenderer>().material;
		}

		private void Start()
		{
			_main = Camera.main;
		}

		private void Update()
		{
			if (_crosshair != null)
			{
				Vector2 pos = _main.ScreenToWorldPoint(Input.mousePosition);
				_crosshair.transform.position = pos;
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
	}
}
