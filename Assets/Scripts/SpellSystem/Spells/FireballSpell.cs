using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.SpellSystem.Spells
{
	public class FireballSpell : Spell
	{
		[SerializeField, Required]
		private GameObject MissileFX;

		[SerializeField]
		private float Speed = 10f;

		[SerializeField]
		private float Damage = 16f;

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
				damage.TakeDamage(new DamageInfo(Damage, Caster.Damage, Element));
				Destroy(missile.gameObject);
			}
		}
	}
}
