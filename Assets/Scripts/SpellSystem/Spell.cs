using FMODUnity;
using Quinn.DamageSystem;
using Quinn.SpellSystem.Spells;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn.SpellSystem
{
	public abstract class Spell : MonoBehaviour
	{
		public const float BufferCharge = 1f;

		[field: SerializeField]
		public float MaxCharge { get; private set; } = 100f;

		[field: SerializeField]
		public bool CanOvercharge { get; private set; }

		[field: SerializeField, ShowIf(nameof(CanOvercharge))]
		public float MaxOvercharge { get; private set; } = 200f;

		[field: SerializeField]
		public float TargetRadius { get; private set; } = 0.2f;

		[field: SerializeField]
		public ElementType Element { get; private set; }

		[field: SerializeField]
		public float CasterKnockbackSpeed { get; private set; } = 5f;

		[SerializeField, BoxGroup("Audio")]
		private EventReference CastSound;

		protected SpellCaster Caster { get; private set; }
		protected float BaseDamage { get; private set; }

		public event Action OnDestroyed;

		private Damage _casterDamage;

		private void Update()
		{
			OnUpdate();
		}

		public bool IsCharged(float charge)
		{
			return Mathf.Abs(charge - MaxCharge) < BufferCharge;
		}

		public bool IsOvercharged(float charge)
		{
			return Mathf.Abs(charge - MaxOvercharge) < BufferCharge;
		}

		public virtual void Cast(SpellCaster caster, float charge, float baseDamage, Vector2 target)
		{
			Caster = caster;
			_casterDamage = caster.GetComponent<Damage>();
			BaseDamage = baseDamage;

			OnCast(charge, target);
			AudioManager.Play(CastSound, caster.transform.position);
		}

		protected virtual Missile[] SpawnMissile(Vector2 position, Vector2 dir, 
			MissileInfo info, GameObject attached = null,
			int count = 1, float spreadAngle = 0f, SpreadPattern pattern = SpreadPattern.Random)
		{
			var missiles = new Missile[count];
			float angleDelta = spreadAngle / count;

			for (int i = 0; i < count; i++)
			{
				float angle;
				if (pattern == SpreadPattern.Sequential)
				{
					angle = angleDelta * i;
					angle += angleDelta / 2f;
					if ((count % 2) == 0) angle -= angleDelta / 2f;
				}
				else
				{
					angle = UnityEngine.Random.Range(0f, spreadAngle);
				}
				angle -= spreadAngle / 2f;

				const string key = "Missile.prefab";
				var instance = Addressables.InstantiateAsync(key, position, Quaternion.identity)
					.WaitForCompletion();

				var missile = instance.GetComponent<Missile>();
				missiles[i] = missile;

				if (attached != null)
				{
					var attachment = Instantiate(attached, instance.transform);
					missile.Attached = attachment;
				}

				float rootAngle = Mathf.Atan2(dir.y, dir.x);
				rootAngle += angle * Mathf.Deg2Rad;
				var finalDir = new Vector2(Mathf.Cos(rootAngle), Mathf.Sin(rootAngle));

				missile.Launch(finalDir, info);
			}

			return missiles;
		}

		protected virtual bool CanDamage(GameObject gameObject, out Damage damage)
		{
			if (gameObject.TryGetComponent(out damage))
			{
				return damage.CanTakeDamage(_casterDamage.Team);
			}

			return false;
		}

		public virtual void OnCasterDeath() { }

		protected virtual void OnCast(float charge, Vector2 target) { }
		protected virtual void OnUpdate() { }

		protected virtual void OnDestroy()
		{
			OnDestroyed?.Invoke();
		}
	}
}
