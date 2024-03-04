using FMODUnity;
using Quinn.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI.Enemies
{
	public class HeadlessKnight : Enemy
	{
		[SerializeField]
		private string Title = "Headless Ghost Knight";

		[SerializeField]
		private Dialogue OpeningDialogue;

		[SerializeField, Required, BoxGroup("References")]
		private Transform Head;

		[SerializeField, BoxGroup("References")]
		private EventReference BossMusic;

		[SerializeField, Required, BoxGroup("References")]
		private Trigger StartTrigger;

		[SerializeField, Required, BoxGroup("References")]
		private Door[] Doors;

		protected override void Awake()
		{
			base.Awake();
			DisableDamage = true;
		}

		protected override void Update()
		{
			base.Update();
		}

		protected override void OnRegisterStates()
		{

		}

		protected override void OnDeath()
		{
			base.OnDeath();

			foreach (var door in Doors)
			{
				door.Open();
			}
		}
	}
}
