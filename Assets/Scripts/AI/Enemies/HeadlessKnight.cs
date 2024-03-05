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

		[SerializeField, Required, BoxGroup("References")]
		private GameObject HeadlessKnightHead;

		public void SpawnHeadMissile()
		{
			var instance = Instantiate(HeadlessKnightHead, Head.position, Head.rotation, transform);
			instance.GetComponent<HeadlessKnightHead>().Origin = Head;
		}

		protected override void Start()
		{
			base.Start();
			Animator.SetTrigger("TossHead");
		}

		protected override void OnRegister()
		{
			
		}
	}
}
