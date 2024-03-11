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
		[SerializeField]
		private string Title = "Fallen Greatsword Knight";

		[SerializeField, BoxGroup("AI")]
		private float DashSpeed = 15f, DashStoppingDistance = 2f;

		[SerializeField, BoxGroup("AI")]
		private AIRand SwingCooldown = (3f, 1f);

		[SerializeField, BoxGroup("References"), Required]
		private Transform Head;

		[SerializeField, BoxGroup("References"), Required]
		private Transform SwordPivot;

		[SerializeField, BoxGroup("References"), Required]
		private WeaponDamage WeaponHitbox;

		[SerializeField, BoxGroup("SFX")]
		private EventReference SwingSound;

		private float _headDir = 1f;

		protected override void Start()
		{
			base.Start();

			HUDUI.Instance.ShowBossBar(Title, GetComponent<Health>());
			WeaponHitbox.enabled = false;
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
			// Spin attack
			new Tasks.MoveTo(Player.transform, 5f, timeout: 8f)
			{
				Services = new() { new Services.PlayAnim("IsSpinning") },
				Conditionals = new()
				{
					new Conditionals.SecondPhase(),
					new Conditionals.Cooldown(25f)
				}
			},
			// Projectile attack
			new Composites.Sequence(new Conditionals.Chance(0.3f), new Conditionals.Cooldown(7f))
			{
				new Tasks.PlayAnim("ArcSlash"),
				new Tasks.Wait(2f, 0.5f)
			},
			// Close melee attack
			new Composites.Sequence()
			{
				new Tasks.MoveTo(Player.transform, 4f, 1.5f, 5f)
				{
					Services = new() { new Services.PlayAnim("IsMoving") }
				},
				new Tasks.PlayAnim("Swing")
			},
			// Dash attack
			new Composites.Sequence()
			{
				new Tasks.MoveTo(Player.transform, 25f, 2f)
				{
					Services = new()
					{
						new Services.PlayAnim("IsDashing"),
					}
				},
				new Tasks.PlayAnim("QuickSwing"),
				new Tasks.Wait(1f, 0.3f)
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
			WeaponHitbox.enabled = true;
		}

		public void OnSwingEnd()
		{
			Movement.ResetMoveSpeed();
			SwordPivot.DORotate(Vector2.right, 1f);

			WeaponHitbox.enabled = false;
		}

		public void PlaySwingSound()
		{
			AudioManager.Play(SwingSound, SwordPivot.GetChild(0));
		}
	}
}
