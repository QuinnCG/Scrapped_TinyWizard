using FMODUnity;
using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

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

		[SerializeField]
		private GameObject SpawnOnHit;

		[SerializeField]
		private float SpawnOnHitLifespan = 3f;

		[SerializeField]
		private bool DetachParticlesOnHit;

		[SerializeField, EnableIf(nameof(DetachParticlesOnHit))]
		private bool DisableSpritesOnHit = true;

		[SerializeField, Tooltip("0f or below will disable lifespan.")]
		private float DetachedParticlesLifespan = 5f;

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
		}

		private void OnHit(Missile missile, GameObject hit)
		{
			if (CanDamage(hit, out var damage))
			{
				Vector2 dir = missile.Velocity.normalized;
				damage.TakeDamage(new DamageInfo(DamageFactor * BaseDamage, dir, Caster.Damage, Element));
			}
			else if (!hit.IsLayer(GameManager.ObstacleLayerName))
			{
				return;
			}

			if (missile.Attached && DetachParticlesOnHit)
			{
				GameObject attached = missile.Attached;
				var vfx = attached.GetComponentInChildren<VisualEffect>();

				if (vfx)
				{
					vfx.transform.parent = null;
					Destroy(vfx.gameObject, DetachedParticlesLifespan);
				}

				if (DisableSpritesOnHit)
				{
					foreach (var renderer in attached.GetComponents<SpriteRenderer>())
					{
						renderer.enabled = false;
					}
				}
			}

			if (SpawnOnHit != null)
			{
				var instance = Instantiate(SpawnOnHit, missile.transform.position, Quaternion.identity);

				if (SpawnOnHitLifespan > 0f)
				{
					Destroy(instance, SpawnOnHitLifespan);
				}
			}

			Destroy(missile.gameObject);
		}
	}
}
