using DG.Tweening;
using FMODUnity;
using Quinn.DamageSystem;
using Quinn.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI.Enemies
{
	public class GreatswordKnight : Enemy
	{
		[SerializeField, BoxGroup("AI")]
		private float DashSpeed = 15f, DashStoppingDistance = 2f;

		[SerializeField, BoxGroup("AI")]
		private AIRand SwingCooldown = (3f, 1f);

		[SerializeField, BoxGroup("References"), Required]
		private Transform Head;

		[SerializeField, BoxGroup("References"), Required]
		private Transform SwordPivot;

		[SerializeField, BoxGroup("SFX")]
		private EventReference SwingSound;

		private float _headDir = 1f;

		protected override void Start()
		{
			base.Start();
			HUDUI.Instance.ShowBossBar("Greatsword Knight", GetComponent<Health>());
		}

		protected override void Update()
		{
			base.Update();

			// Make head face player even if body is not.
			if (TargetDir.x != 0f) _headDir = Mathf.Sign(TargetDir.x);
			Head.transform.localScale = new Vector3(_headDir * transform.localScale.x, 1f, 1f);
		}

		protected override Tree ConstructTree() => new()
		{
			new Composites.Sequence() 
			{
				new Tasks.MoveTo(Player.transform, DashSpeed, DashStoppingDistance)
				{
					Services = new() { new Services.PlayAnim("IsDashing") }
				},
				new Tasks.TriggerAnim("Swing"),
				new Tasks.Wait(SwingCooldown)
			}
		};

		public void OnSwingCharge()
		{
			SwordPivot.DOKill();
			SwordPivot.DORotate(TargetDir, 0.2f);

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
