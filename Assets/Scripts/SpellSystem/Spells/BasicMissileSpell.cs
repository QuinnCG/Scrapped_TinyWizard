using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.SpellSystem.Spells
{
	public class BasicMissileSpell : Spell
	{
		[SerializeField, Required]
		private GameObject MissileFX;

		[SerializeField]
		private float Speed = 10f;

		[SerializeField]
		private float DamageFactor = 1f;

		[SerializeField]
		private float HitRadius = 0.5f;

		protected override void OnCast(float charge, Vector2 target)
		{
			Vector2 pos = Caster.SpellOrigin.position;
			Vector2 dir = (target - (Vector2)transform.position).normalized;
			var info = new MissileInfo()
			{
				Speed = Speed,
				HitRadius = HitRadius
			};

			var missile = SpawnMissile(pos, dir, info, MissileFX);
			missile.OnHit += hit => OnHit(missile, hit);
			missile.OnDestroyed += () =>
			{
				Destroy(missile.Attached, 2f);
			};
		}

		private void OnHit(Missile missile, GameObject hit)
		{
			if (CanDamage(hit, out var damage))
			{
				Vector2 dir = missile.Velocity.normalized;
				damage.TakeDamage(new DamageInfo(DamageFactor * BaseDamage, dir, Caster.Damage, Element));

				Destroy(missile.Attached, 0f);
				Destroy(missile.gameObject);
			}
		}
	}
}
