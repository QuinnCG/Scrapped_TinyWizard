using Quinn.DialogueSystem;
using Quinn.AI.States;
using Sirenix.OdinInspector;
using UnityEngine;
using FMODUnity;

namespace Quinn.AI.Enemies
{
	public class HeadlessKnight : Enemy
	{
		[SerializeField]
		private Dialogue OpeningDialogue;

		[SerializeField, Required, BoxGroup("References")]
		private Transform Head;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject HeadlessKnightHead;

		[SerializeField, BoxGroup("Audio")]
		private EventReference HeadTossSound;

		protected override void Update()
		{
			base.Update();
			Animator.SetBool("IsMoving", Movement.IsMoving);
		}

		public void SpawnHeadMissile()
		{
			var instance = Instantiate(HeadlessKnightHead, Head.position, Head.rotation, transform);
			instance.GetComponent<HeadlessKnightHead>().Origin = Head;

			AudioManager.Play(HeadTossSound, instance.transform);
		}

		protected override void OnRegister()
		{
			var chase = new MoveTo(Player.transform, timeout: 5f, stoppingDistance: 2f);
			var tossHead = new PlayAnimation("TossHead");
			var flee = new StateSequence(this, new DashAway(Player.transform), tossHead);

			SetStart(chase);

			Connect(chase, tossHead, exit => exit);
			Connect(tossHead, chase, exit => exit && TargetDst > 3f);
			Connect(tossHead, flee, exit => exit);
			Connect(flee, chase, exit => exit);
		}
	}
}
