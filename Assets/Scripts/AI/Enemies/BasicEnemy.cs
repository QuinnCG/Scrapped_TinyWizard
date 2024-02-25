using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI.Enemies
{
	public class BasicEnemy : Enemy
	{
		[SerializeField, BoxGroup("Senses")]
		private float SpotPlayerRadius = 10f;

		[SerializeField, BoxGroup("Movement")]
		private float StoppingDistance = 0.5f;

		[SerializeField, BoxGroup("Movement")]
		private bool DoesHop;

		[SerializeField, BoxGroup("Movement"), ShowIf(nameof(DoesHop))]
		private float MaxHopDistance = 2f;

		[SerializeField, BoxGroup("Movement"), ShowIf(nameof(DoesHop))]
		private float HopHeight = 1f;

		[SerializeField, BoxGroup("Movement"), ShowIf(nameof(DoesHop))]
		private float HopDuration = 0.3f;

		[SerializeField, BoxGroup("Movement"), ShowIf(nameof(DoesHop))]
		private float HopCooldown = 0.5f;

		[SerializeField, BoxGroup("Patrolling")]
		private float PatrolRadius = 8f;

		[SerializeField, BoxGroup("Patrolling")]
		private bool PatrolOrigin = true;

		[SerializeField, BoxGroup("Patrolling"), MinMaxSlider(0f, 10f, ShowFields = true)]
		private Vector2 PatrolWaitDuration = new();

		private bool _spottedPlayer;

		private Vector2 _origin;
		private Vector2 _targetPos;
		private float _nextPatrolTime;
		private float _nextHopTime;

		protected override void Awake()
		{
			base.Awake();

			_origin = transform.position;
			_targetPos = _origin;
		}

		protected override void Update()
		{
			LookForPlayer();

			if (_spottedPlayer)
			{
				_targetPos = PlayerPos;

				if (PlayerDist > 0.3f)
				{
					Move();
				}
			}
			else
			{
				float dst = Vector2.Distance(transform.position, _targetPos);
				if (dst < StoppingDistance)
				{
					Vector2 origin = PatrolOrigin ? _origin : transform.position;
					_targetPos = origin + (PatrolRadius * Random.insideUnitCircle / 2f);

					_nextPatrolTime = Time.time + Random.Range(PatrolWaitDuration.x, PatrolWaitDuration.y);
				}

				if (Time.time > _nextPatrolTime)
				{
					Move();
				}
			}
		}

		private void LookForPlayer()
		{
			if (!_spottedPlayer && PlayerDist < SpotPlayerRadius)
			{
				if (HasLineOfSight(PlayerPos))
				{
					_spottedPlayer = true;
				}
			}
		}

		private void Move()
		{
			if (DoesHop)
			{
				if (!IsJumping && Time.time > _nextHopTime)
				{
					var difference = _targetPos - Position;
					float dst = difference.magnitude;
					Vector2 dir = difference.normalized;

					Vector2 target = Position + (dir * Mathf.Min(dst, MaxHopDistance));
					var tween = JumpTo(target, HopHeight, HopDuration);

					tween.onComplete += () =>
					{
						_nextHopTime = Time.time + HopCooldown;
					};

					Movement.SetFacingDirection(dir.x);
				}
			}
			else
			{
				Movement.MoveTowards(_targetPos);
			}
		}
	}
}
