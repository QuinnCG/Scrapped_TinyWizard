using UnityEngine;

namespace Quinn.DamageSystem
{
	[System.Serializable]
	public class DamageInfo
	{
		public float Damage;
		public Vector2 Direction;
		public Damage Source;
		public ElementType Element;
		public float KnockbackScale;

		public DamageInfo(float damage, Vector2 direction, Damage source, ElementType element, float knockbackScale = 1f)
		{
			Damage = damage;
			Direction = direction;
			Source = source;
			Element = element;
			KnockbackScale = knockbackScale;
		}
	}
}
