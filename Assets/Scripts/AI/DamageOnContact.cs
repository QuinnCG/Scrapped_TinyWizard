using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(CapsuleCollider2D))]
	[RequireComponent(typeof(Damage))]
	public class DamageOnContact : MonoBehaviour
	{
		[SerializeField]
		private float Damage = 18f;

		[SerializeField]
		private ElementType Element;

		private Damage _damage;
		private CapsuleCollider2D _collider;

		private void Awake()
		{
			_damage = GetComponent<Damage>();
			_collider = GetComponent<CapsuleCollider2D>();
		}

		private void FixedUpdate()
		{
			var colliders = Physics2D.OverlapCapsuleAll(
				_collider.bounds.center,
				_collider.size,
				_collider.direction,
				0f,
				LayerMask.GetMask("Character"));

			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(out Damage damage)
					&& damage.CanTakeDamage(_damage.Team))
				{
					Vector2 dir = (collider.bounds.center - _collider.bounds.center).normalized;
					damage.TakeDamage(new DamageInfo(Damage, dir, _damage, Element));
				}
			}
		}
	}
}
