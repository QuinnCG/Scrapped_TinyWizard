using Sirenix.OdinInspector;
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

		[field: SerializeField, Tooltip("Low means more accurate.")]
		public float TargetRadius { get; private set; } = 0.2f;

		[field: SerializeField]
		public ElementType Element { get; private set; }

		protected SpellCaster Caster { get; private set; }
		protected float BaseDamage { get; private set; }

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

		public void Cast(SpellCaster caster, float charge, float baseDamage, Vector2 target)
		{
			Caster = caster;
			_casterDamage = caster.GetComponent<Damage>();
			BaseDamage = baseDamage;

			target += Random.insideUnitCircle * (TargetRadius / 2f);
			OnCast(charge, target);
		}

		protected Missile SpawnMissile(Vector2 position, Vector2 dir, MissileInfo info, GameObject attached = null)
		{
			const string key = "Missile.prefab";
			var instance = Addressables.InstantiateAsync(key, position, Quaternion.identity)
				.WaitForCompletion();

			var missile = instance.GetComponent<Missile>();

			if (attached != null)
			{
				var a = Instantiate(attached, instance.transform);
				missile.Attached = a;
			}

			missile.Launch(dir, info);

			return missile;
		}

		protected bool CanDamage(GameObject gameObject, out Damage damage)
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
	}
}
