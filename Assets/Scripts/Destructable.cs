using Quinn.DamageSystem;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Damage))]
	public class Destructable : MonoBehaviour
	{
		[SerializeField]
		private Sprite Remains;

		private SpriteRenderer _renderer;
		private bool _damaged;

		private void Awake()
		{
			_renderer = GetComponent<SpriteRenderer>();
			GetComponent<Damage>().OnDamaged += OnDamaged;
		}

		private void OnDamaged(DamageInfo info, DamageEfficiencyType type)
		{
			if (_damaged) return;
			_damaged = true;

			if (Remains == null)
			{
				_renderer.sprite = null;

				Destroy(gameObject, 3f);
			}
			else
			{
				_renderer.sprite = Remains;
			}

			GetComponent<Collider2D>().enabled = false;
			GetComponentInChildren<VisualEffect>().enabled = true;
		}
	}
}
