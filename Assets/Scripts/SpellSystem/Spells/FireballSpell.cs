using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.SpellSystem.Spells
{
	public class FireballSpell : Spell
	{
		[SerializeField, Required]
		private GameObject MissileFX;

		protected override void OnCast(float charge)
		{
			
		}

		protected override void OnUpdate()
		{
			
		}

		public override void OnCasterDeath()
		{
			
		}
	}
}
