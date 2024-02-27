using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI.Enemies
{
	public class BasicChargeEnemy : Enemy
	{
		[SerializeField, BoxGroup("Senses")]
		private float SpotPlayerRadius = 10f;

		[SerializeField, BoxGroup("Movement")]
		private float StoppingDistance = 0.2f;

		[SerializeField, BoxGroup("Movement/Perlin")]
		private bool UsePerlin;

		[SerializeField, BoxGroup("Movement/Perlin"), ShowIf(nameof(UsePerlin))]
		private float PerlinBias = 0.5f;

		[SerializeField, BoxGroup("Movement/Perlin"), ShowIf(nameof(UsePerlin))]
		private float PerlinFrequency = 1f;

		[SerializeField, BoxGroup("Movement/Perlin"), ShowIf(nameof(UsePerlin))]
		private float PerlinAmplitude = 4f;

		[SerializeField, BoxGroup("Patrolling")]
		private float PatrolRadius = 8f;

		[SerializeField, BoxGroup("Patrolling")]
		private bool PatrolOrigin = true;

		[SerializeField, BoxGroup("Patrolling"), MinMaxSlider(0f, 10f, ShowFields = true)]
		private Vector2 PatrolWaitDuration = new();
		protected Vector2 TargetPos { get; set; }

		protected bool IsPlayerSpotted { get; private set; }

		private Vector2 _origin;
		private float _nextPatrolTime;

		protected override void Awake()
		{
			base.Awake();

			_origin = transform.position;
			TargetPos = _origin;
		}

		protected override void Update()
		{
			if (!IsPlayerSpotted)
			{
				IsPlayerSpotted = ShouldSpotPlayer();
			}

			if (IsPlayerSpotted)
			{
				TargetPos = PlayerPos;

				if (PlayerDist > StoppingDistance)
				{
					Move(UsePerlin);
				}
			}
			else
			{
				float dst = Vector2.Distance(transform.position, TargetPos);
				if (dst < StoppingDistance)
				{
					Vector2 origin = PatrolOrigin ? _origin : transform.position;
					TargetPos = origin + (PatrolRadius * Random.insideUnitCircle / 2f);

					_nextPatrolTime = Time.time + Random.Range(PatrolWaitDuration.x, PatrolWaitDuration.y);
				}

				if (Time.time > _nextPatrolTime)
				{
					Move();
				}
			}

			Animator.SetBool("IsMoving", Movement.IsMoving);
		}

		protected virtual bool ShouldSpotPlayer()
		{
			if (!IsPlayerSpotted && PlayerDist < SpotPlayerRadius)
			{
				if (HasLineOfSight(PlayerPos))
				{
					return true;
				}
			}

			return false;
		}

		protected virtual void Move(bool usePerlin = false)
		{
			if (usePerlin)
			{
				float value = Mathf.PerlinNoise1D(Time.time * PerlinFrequency);
				value = (value - 0.5f) * 2f;
				value *= PerlinAmplitude;

				Vector2 targetDir = (TargetPos - Position).normalized;
				var perpen = new Vector2(targetDir.y, -targetDir.x);

				Vector2 finalDir = Vector2.Lerp(targetDir, perpen.normalized * value, PerlinBias);

				Movement.Move(finalDir);
				Movement.SetFacingDirection(targetDir.x);
			}
			else
			{
				Movement.MoveTowards(TargetPos);
			}
		}
	}
}
