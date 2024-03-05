using FMODUnity;
using Quinn.AI.States;
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
			var moveTo = new MoveTo(Player.transform);
			var wait = new Wait(2f, 5f);
			var dashAway = new DashAway(Player.transform);

			SetStart(moveTo);

			Connect(moveTo, dashAway, _ => PlayerDst < 2f);
			Connect(dashAway, wait, exit => exit);
			Connect(wait, moveTo, exit => exit);
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
