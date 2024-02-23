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

		protected SpellCaster Caster { get; private set; }

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

		public void Cast(SpellCaster caster, float charge)
		{
			Caster = caster;
			_casterDamage = caster.GetComponent<Damage>();

			OnCast(charge);
		}

		protected Missile SpawnMissile(Vector2 position, Vector2 dir, MissileInfo info, GameObject attached = null)
		{
			const string key = "Missile.prefab";
			var instance = Addressables.InstantiateAsync(key, position, Quaternion.identity)
				.WaitForCompletion();

			if (attached != null)
			{
				Instantiate(attached, instance.transform);
			}

			var missile = instance.GetComponent<Missile>();
			missile.Launch(dir, info);

			return missile;
		}

		protected bool CanDamage(GameObject gameObject)
		{
			if (gameObject.TryGetComponent(out Damage damage))
			{
				return damage.CanTakeDamage(_casterDamage.Team);
			}

			return false;
		}

		public virtual void OnCasterDeath() { }

		protected virtual void OnCast(float charge) { }
		protected virtual void OnUpdate() { }
	}
}
