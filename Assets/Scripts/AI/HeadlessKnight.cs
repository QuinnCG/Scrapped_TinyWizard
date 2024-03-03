using Quinn.DamageSystem;
using Quinn.DialogueSystem;
using Quinn.UI;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Quinn.AI
{
	public class HeadlessKnight : Enemy
	{
		[SerializeField]
		private string Title = "Headless Ghost Knight";

		[SerializeField]
		private Dialogue OpeningDialogue;

		[SerializeField, Required, BoxGroup("References")]
		private Transform Head;

		[SerializeField, Required, BoxGroup("References")]
		private Trigger StartTrigger;

		[SerializeField, Required, BoxGroup("References")]
		private Door[] Doors;

		protected override void Awake()
		{
			base.Awake();

			StartTrigger.OnTrigger += () => SetStartState(OnStart);
			BlockGlobalConnections = true;
			DisableDamage = true;
		}

		protected override void OnRegisterStates()
		{
			Register(OnStart, OnEngage);
			Connect(OnEngage, _ => CurrentState == OnStart);
		}

		protected override void OnDeath()
		{
			base.OnDeath();

			foreach (var door in Doors)
			{
				door.Open();
			}
		}

		private IEnumerator StartSequence()
		{
			foreach (var door in Doors)
			{
				door.Close();
			}

			var manager = DialogueManager.Instance;
			manager.Display(OpeningDialogue);

			yield return new WaitUntil(() => !manager.InDialogue);

			BlockGlobalConnections = false;
			DisableDamage = false;
			HUDUI.Instance.ShowBossBar(Title, GetComponent<Health>());
		}

		private bool OnStart(bool isStart)
		{
			if (isStart)
			{
				StartCoroutine(StartSequence());
			}

			return false;
		}

		private bool OnEngage(bool isStart)
		{
			if (PlayerDst > 0.2f)
			{
				Movement.MoveTowards(PlayerPos);
			}

			Animator.SetBool("IsMoving", Movement.IsMoving);
			return true;
		}
	}
}
