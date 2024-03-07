using Quinn.DialogueSystem;
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

		[SerializeField, Required, BoxGroup("References")]
		private GameObject FireSpewSpell;

		[SerializeField, BoxGroup("Audio")]
		private EventReference HeadTossSound;

		private Vector2 _targetPos;

		public void CaptureTargetPos()
		{
			_targetPos = TargetPos;
		}

		public void SpawnHeadMissile()
		{
			var instance = Instantiate(HeadlessKnightHead, Head.position, Head.rotation, transform);

			var head = instance.GetComponent<HeadlessKnightHead>();
			head.Origin = Head;
			head.Target = _targetPos;

			AudioManager.Play(HeadTossSound, instance.transform);
		}

		public void SpewFire()
		{
			Caster.CastSpell(FireSpewSpell, TargetPos);
		}

		protected override Tree ConstructTree()
		{
			//return new Tree()
			//{
			//	new Composites.Sequence(new Conditionals.FarFrom(Player.transform, 3f))
			//	{
			//		new Tasks.MoveTo(Player.transform, 2.25f, timeout: 6f)
			//		{
			//			Services = new() { new Services.PlayAnim("IsMoving") }
			//		},
			//		new Tasks.TriggerAnim("TossHead")
			//	},
			//	new Composites.Sequence()
			//	{
			//		new Tasks.FleeFrom(Player.transform, 15f, 1f, true)
			//		{
			//			Services = new() { new Services.PlayAnim("IsDashing") }
			//		},
			//		new Tasks.Wait(1.5f, 0.5f)
			//	}
			//};

			return new Tree()
			{
				new Composites.Sequence()
				{
					new Tasks.TriggerAnim("TossHead"),
					new Tasks.Wait(3f)
				}
			};
		}
	}
}
