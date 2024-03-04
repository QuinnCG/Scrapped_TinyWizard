using FMODUnity;
using Quinn.DamageSystem;
using Quinn.DialogueSystem;
using Quinn.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace Quinn.AI.Enemies
{
	public class HeadlessKnight : Enemy
	{
		[SerializeField]
		private string Title = "Headless Ghost Knight";

		[SerializeField]
		private Dialogue OpeningDialogue;

		[SerializeField]
		private Vector2 TossHeadCooldown = new(5f, 8f);

		[SerializeField, Required, BoxGroup("References")]
		private Transform Head;

		[SerializeField, BoxGroup("References")]
		private EventReference BossMusic;

		[SerializeField, Required, BoxGroup("References")]
		private Trigger StartTrigger;

		[SerializeField, Required, BoxGroup("References")]
		private Door[] Doors;

		private readonly Timer _tossHeadCooldown = new();
		private readonly Timer _tossHeadDuration = new();

		protected override void Awake()
		{
			base.Awake();
			DisableDamage = true;
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
