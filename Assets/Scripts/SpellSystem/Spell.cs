using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn.SpellSystem
{
	public abstract class Spell : MonoBehaviour
	{
		protected SpellCaster Caster { get; private set; }

		private Damage _casterDamage;

		private void Update()
		{
			OnUpdate();
		}

		public void Cast(SpellCaster caster)
		{
			Caster = caster;
			_casterDamage = caster.GetComponent<Damage>();

			OnCast();
		}

		protected Missile SpawnMissile(Vector2 position, Vector2 dir, MissileInfo info)
		{
			const string key = "Missile.prefab";
			var instance = Addressables.InstantiateAsync(key, position, Quaternion.identity, transform).WaitForCompletion();

			var missile = instance.GetComponent<Missile>();
			missile.Launch(dir, info);

			return missile;
		}

		protected bool CanHitGameObject(GameObject gameObject)
		{
			if (gameObject.TryGetComponent(out Damage damage))
			{
				return damage.CanTakeDamage(_casterDamage.Team);
			}

			return false;
		}

		public virtual void OnCasterDeath() { }

		protected virtual void OnCast() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnHit(GameObject hit) { }
	}
}
