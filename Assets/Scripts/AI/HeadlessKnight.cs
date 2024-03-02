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

		[SerializeField, Required]
		private Transform Head;

		protected override void OnRegisterStates()
		{
			BlockGlobalConnections = true;

			Register(OnStart, OnEngage);
			SetStartState(OnStart);

			Connect(OnEngage, _ => CurrentState == OnStart);
		}

		private IEnumerator StartSequence()
		{
			var manager = DialogueManager.Instance;
			manager.Display(OpeningDialogue);

			yield return new WaitUntil(() => !manager.InDialogue);
			BlockGlobalConnections = false;

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
			Animator.SetBool("IsMoving", true);
			Movement.MoveTowards(PlayerPos);

			return true;
		}
	}
}
