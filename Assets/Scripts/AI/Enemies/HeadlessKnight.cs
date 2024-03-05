using FMODUnity;
using Quinn.AI.States;
using Quinn.DamageSystem;
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

		[SerializeField, BoxGroup("AI")]
		private Meter AggroMeter = new();

		[SerializeField, BoxGroup("AI")]
		private Meter FearMeter = new();

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Update()
		{
			base.Update();

			AggroMeter += Time.deltaTime * 3.5f;
			Animator.SetBool("IsMoving", Movement.IsMoving);

			Debug.Log($"Aggro {AggroMeter} Fear {FearMeter}");
		}

		protected override void OnRegister()
		{
			var moveTo = new MoveTo(Player.transform, 5f);
			var dashAway = new DashAway(Player.transform);

			OnFSMUpdate += current =>
			{
				if (current == moveTo)
				{
					AggroMeter -= Time.deltaTime * 5f;
				}
			};

			RegisterState(moveTo, dashAway);

			RegisterMeter(AggroMeter);
			RegisterMeter(FearMeter);

			RegisterAction(moveTo, (AggroMeter, -25f));
			RegisterAction(dashAway, (FearMeter, -10f));
		}

		protected override void OnDamaged(DamageInfo info, DamageEfficiencyType type)
		{
			FearMeter += 15f;
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
