using Quinn.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using FMODUnity;
using System.Collections;
using Quinn.UI;
using Quinn.DamageSystem;
using UnityEngine.VFX;

namespace Quinn.AI.Enemies
{
	public class HeadlessKnight : Enemy
	{
		[SerializeField]
		private Dialogue OpeningDialogue;

		[SerializeField]
		private string Title = "Headless Knight";

		[SerializeField, Required, BoxGroup("References")]
		private Transform Head;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject HeadlessKnightHead;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject FireSpewSpell;

		[SerializeField, Required, BoxGroup("References")]
		private VisualEffect DashTrail;

		[SerializeField, BoxGroup("Audio")]
		private EventReference BossMusic;

		[SerializeField, BoxGroup("Audio")]
		private EventReference HeadTossSound;

		[SerializeField, BoxGroup("Audio")]
		private EventReference DashSound;

		private Vector2 _targetPos;

		protected override void Start()
		{
			base.Start();

			if (GameManager.Instance.IsEasyMode)
			{
				var hp = GetComponent<Health>();
				hp.SetMax(hp.Max * 0.6f);
			}

			StartCoroutine(IntroSequence());
		}

		public void CaptureTargetPos()
		{
			if (GameManager.Instance.IsEasyMode)
			{
				_targetPos = TargetPos;
			}
			else
			{
				float outDir = 0.9f / 2f;
				Vector2 estimatedPos = TargetPos + (TargetVel.normalized * TargetVel.magnitude * outDir);

				_targetPos = estimatedPos;
			}
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

		public void PlayDashSound()
		{
			AudioManager.Play(DashSound, transform.position);
		}

		protected override Tree ConstructTree()
		{
			return new Tree()
			{
				new Composites.Sequence(
					new Conditionals.Chance(0.7f),
					new Conditionals.Cooldown(5f, 1f, true),
					new Conditionals.SecondPhase())
				{
					new Tasks.PlaySound(DashSound, transform),
					new Tasks.MoveTowards(Player.transform, 15f, true)
					{
						Conditionals = new()
						{
							new Conditionals.Timer(1f, 0.25f)
						},
						Services = new()
						{
							new Services.PlayAnim("IsDashing"),
							new Services.Custom(
								() => DashTrail.SetBool("Enable", true),
								() => DashTrail.SetBool("Enable", false))
						}
					}
				},
				new Composites.Sequence()
				{
					new Composites.Succeed()
					{
						new Tasks.MoveTo(Player.transform, 3f, 2f, 4.5f)
						{
							Services = new() { new Services.PlayAnim("IsMoving") }
						}
					},
					new Tasks.FaceTarget(Player.transform),
					new Composites.Selector()
					{
						new Tasks.TriggerAnim("SpewFire")
						{
							Conditionals = new()
							{
								new Conditionals.Chance(0.3f),
								new Conditionals.FirstPhase()
							}
						},
						new Tasks.TriggerAnim("SpewFire")
						{
							Conditionals = new()
							{
								new Conditionals.Chance(0.5f),
								new Conditionals.SecondPhase()
							}
						},
						new Composites.Repeat(1, 2, new Conditionals.FirstPhase())
						{
							new Tasks.TriggerAnim("TossHead")
						},
						new Composites.Repeat(2, 3, new Conditionals.SecondPhase())
						{
							new Tasks.TriggerAnim("TossHead")
						}
					},
					new Tasks.Wait(0.5f, 0.2f)
					{
						Conditionals = new() { new Conditionals.FirstPhase() }
					},
					new Tasks.Wait(0.3f, 0.1f)
					{
						Conditionals = new() { new Conditionals.SecondPhase() }
					}
				}
			};
		}

		private IEnumerator IntroSequence()
		{
			UpdateTree = false;
			yield return DialogueManager.Instance.Display(OpeningDialogue);

			UpdateTree = true;
			HUDUI.Instance.ShowBossBar(Title, GetComponent<Health>());
			AudioManager.Instance.PlayBossMusic(BossMusic, () => IsDead);
		}
	}
}
