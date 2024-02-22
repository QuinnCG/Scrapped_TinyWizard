using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn.SpellSystem
{
	public abstract class Spell : MonoBehaviour
	{
		protected ICaster Caster { get; private set; }
		protected GameObject CasterGameObject { get; private set; }

		private Damage _casterDamage;

		private void Update()
		{
			OnUpdate();
		}

		public void Cast(ICaster caster)
		{
			Caster = caster;
			CasterGameObject = (caster as MonoBehaviour).gameObject;
			_casterDamage = CasterGameObject.GetComponent<Damage>();

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

		protected abstract void OnCast();
		protected abstract void OnUpdate();
		protected abstract void OnHit(GameObject hit);
	}
}
