using UnityEngine;

namespace Quinn.DamageSystem
{
	[RequireComponent(typeof(Collider2D))]
	[ExecuteAlways]
	public class WeaponDamage : MonoBehaviour
	{
		[SerializeField]
		private float Damage = 40f;

		[SerializeField]
		private ElementType Element;

		[SerializeField]
		private float KnockbackFactor = 1f;

		private Collider2D _collider;
		private Damage _damage;

		private void Start()
		{
			_collider = GetComponent<Collider2D>();
			_damage = GetComponentInParent<Damage>();
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				_collider.isTrigger = true;
				gameObject.layer = 0;
			}
#endif
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!enabled) return;

			if (collision.TryGetComponent(out Damage damage))
			{
				if (damage.Team != _damage.Team)
				{
					Vector2 dir = damage.transform.position - transform.position;
					dir.Normalize();

					damage.TakeDamage(new DamageInfo(Damage, dir, _damage, Element, KnockbackFactor));
				}
			}
		}
	}
}
