using FMODUnity;
using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace Quinn.SpellSystem.Spells
{
	public class BasicMissileSpell : Spell
	{
		[SerializeField, Required, BoxGroup("References")]
		private GameObject MissileFX;

		[SerializeField, BoxGroup("References")]
		private EventReference HitSound;

		[SerializeField, BoxGroup("Stats")]
		private float Speed = 10f;

		[SerializeField, BoxGroup("Stats")]
		private float DamageFactor = 1f;

		[SerializeField, BoxGroup("Stats")]
		private float AccuracyRadius = 0.2f;

		[SerializeField, BoxGroup("Stats")]
		private float HitRadius = 0.5f;

		[SerializeField, BoxGroup("Stats")]
		private int Count = 1;

		[SerializeField, BoxGroup("Behavior")]
		private float SpreadAngle = 0f;

		[SerializeField, BoxGroup("Behavior")]
		private SpreadPattern SpreadPattern = SpreadPattern.Random;

		[SerializeField, BoxGroup("Behavior")]
		private GameObject SpawnOnHit;

		[SerializeField, BoxGroup("Behavior"), EnableIf(nameof(SpawnOnHit))]
		private float SpawnOnHitLifespan = 3f;

		[SerializeField, BoxGroup("Behavior")]
		private bool DetachParticlesOnHit;

		[SerializeField, Tooltip("0f or below will disable lifespan."), BoxGroup("Behavior")]
		private float DetachedParticlesLifespan = 5f;

		[SerializeField, EnableIf(nameof(DetachParticlesOnHit)), BoxGroup("Behavior")]
		private bool DisableSpritesOnHit = true;

		[SerializeField, EnableIf(nameof(DetachParticlesOnHit)), BoxGroup("Behavior")]
		private bool DisableLightOnHit = true;

		private readonly List<Missile> _missiles = new();

		protected override void OnCast(float charge, Vector2 target)
		{
			Vector2 pos = Caster.SpellOrigin.position;
			Vector2 dir = (target - (Vector2)Caster.SpellOrigin.position).normalized;
			var info = new MissileInfo()
			{
				Speed = Speed,
				HitRadius = HitRadius
			};

			target += Random.insideUnitCircle / 2f * AccuracyRadius;

			var missiles = SpawnMissile(pos, dir, info, MissileFX, Count, SpreadAngle, SpreadPattern);
			_missiles.AddRange(missiles);

			foreach (var missile in missiles)
			{
				missile.OnHit += hit => OnHit(missile, hit);
			}
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
					foreach (var renderer in attached.GetComponentsInChildren<SpriteRenderer>())
					{
						renderer.enabled = false;
					}
				}

				if (DisableLightOnHit)
				{
					foreach (var light in attached.GetComponentsInChildren<Light2D>())
					{
						light.enabled = false;
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

			AudioManager.Play(HitSound, missile.transform.position);

			// Destroy spell instance if no missiles remain.
			_missiles.Remove(missile);
			Destroy(missile.gameObject);

			if (_missiles.Count == 0)
			{
				Destroy(gameObject);
			}
		}
	}
}
