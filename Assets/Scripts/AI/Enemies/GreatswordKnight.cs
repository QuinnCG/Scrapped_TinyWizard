using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI.Enemies
{
	public class GreatswordKnight : Enemy
	{
		[SerializeField, BoxGroup("References"), Required]
		private Transform Head;

		[SerializeField, BoxGroup("References"), Required]
		private Transform SwordPivot;

		[SerializeField, BoxGroup("SFX")]
		private EventReference SwingSound;

		private float _headDir = 1f;

		protected override void Update()
		{
			base.Update();

			// Make head face player even if body is not.
			if (PlayerDir.x != 0f) _headDir = Mathf.Sign(PlayerDir.x);
			Head.transform.localScale = new Vector3(_headDir * transform.localScale.x, 1f, 1f);
		}

		protected override Tree ConstructTree() => new()
		{
			new Tasks.Log("!")
		};

		public void OnSwingCharge()
		{
			SwordPivot.DOKill();
			SwordPivot.DORotate(PlayerDir, 0.2f);

			Movement.MoveSpeed = 1f;
		}

		public void OnSwing()
		{
			Movement.MoveSpeed = 14f;
		}

		public void OnSwingEnd()
		{
			Movement.ResetMoveSpeed();
			SwordPivot.DORotate(Vector2.right, 1f);
		}

		public void PlaySwingSound()
		{
			AudioManager.Play(SwingSound, SwordPivot.GetChild(0));
		}
	}
}
