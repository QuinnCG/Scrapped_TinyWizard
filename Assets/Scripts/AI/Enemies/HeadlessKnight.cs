using Quinn.DialogueSystem;
using Quinn.AI.States;
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

		[SerializeField, Required, BoxGroup("References")]
		private GameObject HeadlessKnightHead;

		protected override void Update()
		{
			base.Update();
			Animator.SetBool("IsMoving", Movement.IsMoving);
		}

		public void SpawnHeadMissile()
		{
			var instance = Instantiate(HeadlessKnightHead, Head.position, Head.rotation, transform);
			instance.GetComponent<HeadlessKnightHead>().Origin = Head;
		}

		protected override void OnRegister()
		{
			var chase = new MoveTo(Player.transform, timeout: 5f, stoppingDistance: 2f);
			var tossHead = new PlayAnimation("TossHead");
			var flee = new Sequence(this, new DashAway(Player.transform), tossHead);

			SetStart(chase);

			Connect(chase, tossHead, exit => exit);
			Connect(tossHead, chase, exit => exit && PlayerDst > 3f);
			Connect(tossHead, flee, exit => exit);
			Connect(flee, chase, exit => exit);
		}
	}
}
