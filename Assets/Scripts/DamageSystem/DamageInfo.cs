using Unity.VisualScripting;
using UnityEngine;

namespace Quinn.DamageSystem
{
	[System.Serializable, Inspectable]
	public class DamageInfo
	{
		public float Damage;
		public Vector2 Direction;
		public Damage Source;
		public ElementType Element;

		public DamageInfo(float damage, Vector2 direction, Damage source, ElementType element)
		{
			Damage = damage;
			Direction = direction;
			Source = source;
			Element = element;
		}
	}
}
