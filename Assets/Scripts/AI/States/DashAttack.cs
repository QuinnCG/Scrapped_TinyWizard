using System;
using UnityEngine;

namespace Quinn.AI.States
{
	public class DashAttack : State
	{
		enum Phase
		{
			Dash,
			Attack,
			Exit
		}

		public event Action OnAttackStart;

		private Phase _phase;
		private bool _startedAttack;
		private float _attackEndTime;
		private Vector2 _attackMoveDir;

		public Transform Target { get; set; }
		public float DashSpeed { get; set; }
		public float StoppingDistance { get; set; }

		public string DashAnimKey {get; set;}
		public string AttackAnimKey { get; set; }

		protected override void OnEnter()
		{
			_phase = Phase.Dash;
			_startedAttack = false;
		}

		protected override bool OnUpdate()
		{
			switch (_phase)
			{
				case Phase.Dash:
				{
					Agent.Animator.SetBool(DashAnimKey, true);

					Agent.Movement.MoveSpeed = DashSpeed;
					Agent.Movement.MoveTowards(Target.position);

					float dst = Vector2.Distance(Agent.Center, Target.position);
					if (dst < StoppingDistance)
					{
						Agent.Movement.ResetMoveSpeed();
						_phase = Phase.Attack;
					}

					break;
				}
				case Phase.Attack:
				{
					if (!_startedAttack)
					{
						_startedAttack = true;

						Agent.Animator.SetBool(DashAnimKey, false);
						OnAttackStart?.Invoke();

						Agent.Animator.SetTrigger(AttackAnimKey);
						_attackEndTime = Time.time + Agent.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

						_attackMoveDir = (Vector2)Target.position - Agent.Center;
						_attackMoveDir.Normalize();

					}
					else if (Time.time > _attackEndTime)
					{
						_phase = Phase.Exit;
					}

					Agent.Movement.Move(_attackMoveDir);
					break;
				}
			}

			return _phase == Phase.Exit;
		}
	}
}
