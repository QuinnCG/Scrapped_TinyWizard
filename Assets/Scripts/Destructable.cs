using DG.Tweening;
using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Damage))]
	public class Destructable : MonoBehaviour
	{
		[SerializeField]
		private float SpawnOffset = 0.5f;

		[SerializeField]
		private float MinDistance = 0.8f, MaxDistance = 1.25f;

		[SerializeField]
		private float Height = 0.5f;

		[SerializeField]
		private float Duration = 0.5f;

		[SerializeField, Required]
		private Sprite[] Parts;

		[SerializeField]
		private Sprite Remains;

		private SpriteRenderer _renderer;
		private bool _damaged;

		private void Awake()
		{
			_renderer = GetComponent<SpriteRenderer>();
			GetComponent<Damage>().OnDamaged += OnDamaged;
		}

		private static Vector2 GetDirection(int index) => index switch
		{
			0 => new Vector2(-1f, 1f).normalized,
			1 => new Vector2(0f, 1f).normalized,
			2 => new Vector2(1f, 1f).normalized,

			3 => new Vector2(-1f, 0f).normalized,
			4 => new Vector2(0f, 0f).normalized,
			5 => new Vector2(1f, 0f).normalized,

			6 => new Vector2(-1f, -1f).normalized,
			7 => new Vector2(0f, -1f).normalized,
			8 => new Vector2(1f, -1f).normalized,

			_ => throw new System.IndexOutOfRangeException()
		};

		private void OnDamaged(DamageInfo info, DamageEfficiencyType type)
		{
			if (_damaged) return;
			_damaged = true;

			Vector2 dir = info.Direction;
			Vector2 center = _renderer.bounds.center;

			for (int i = 0; i < Parts.Length; i++)
			{
				Vector2 origin = center + (GetDirection(i) * SpawnOffset);
				SpawnPart(Parts[i], origin, dir);
			}

			if (Remains == null)
			{
				_renderer.sprite = null;
			}
			else
			{
				_renderer.sprite = Remains;
			}

			GetComponent<Collider2D>().enabled = false;
		}

		private void SpawnPart(Sprite sprite, Vector2 origin, Vector2 direction)
		{
			var instance = new GameObject("Part");
			instance.transform.parent = transform;

			instance.transform.position = origin;
			instance.AddComponent<SpriteRenderer>().sprite = sprite;

			float dst = Random.Range(MinDistance, MaxDistance);

			instance.transform.DOJump(origin + (direction * dst), Height, 1, Duration)
				.SetEase(Ease.Linear)
				.onComplete += () =>
				{
					Destroy(instance);
				};
		}
	}
}
