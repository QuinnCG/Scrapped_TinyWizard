using Quinn.DamageSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quinn
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Movement : MonoBehaviour
	{
		[field: SerializeField]
		public float MoveSpeed { get; set; } = 5f;

		[SerializeField]
		private float DashSpeed = 12f;

		[field: SerializeField]
		public float DashDuration { get; private set; } = 0.2f;

		[SerializeField]
		private bool DestroyDestructablesOnDash;

		public bool IsMoving { get; private set; }
		public bool IsDashing { get; private set; }

		public float FacingDirection { get; private set; } = 1f;
		public Vector2 Velocity => _rb.velocity;
		public Vector2 MoveDirection { get; private set; }

		public event Action OnDashStart, OnDashEnd;

		private Rigidbody2D _rb;
		private float _defaultMoveSpeed;

		private Vector2 _velocitySum;
		private Vector2 _dashDir = Vector2.right;
		private bool _wasMovingLastFrame;
		private float _dashEndTime;

		private readonly List<Destructable> _overlappedDestructables = new();

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
			_defaultMoveSpeed = MoveSpeed;
		}

		private void Update()
		{
			if (IsDashing)
			{
				_velocitySum += DashSpeed * _dashDir;

				if (DestroyDestructablesOnDash)
				{
					var results = new List<Collider2D>();
					_rb.Overlap(results);

					var dmg = GetComponent<Damage>();

					foreach (var collider in results)
					{
						if (collider.TryGetComponent(out Destructable destructable))
						{
							if (!_overlappedDestructables.Contains(destructable))
							{
								Vector2 dir = (destructable.transform.position - transform.position).normalized;

								collider.GetComponent<Damage>()
									.TakeDamage(
									new DamageInfo(1f, dir, dmg, 0 ));

								_overlappedDestructables.Add(destructable);
							}
						}
					}
				}

				if (Time.time > _dashEndTime)
				{
					StopDash();
				}
			}
		}

		private void LateUpdate()
		{
			_rb.velocity = _velocitySum;
			_velocitySum = Vector2.zero;

			IsMoving = _wasMovingLastFrame;
			_wasMovingLastFrame = false;
		}

		public void Move(Vector2? dir = null)
		{
			if (!IsDashing)
			{
				if (dir.HasValue)
				{
					SetMoveDirection(dir.Value);
				}

				_velocitySum += MoveDirection * MoveSpeed;
				_wasMovingLastFrame = MoveDirection.sqrMagnitude > 0f;

				if (MoveDirection.sqrMagnitude > 0f)
				{
					SetDashDirection(MoveDirection);
					SetFacingDirection(Mathf.Sign(MoveDirection.x));
				}
			}
		}

		public void MoveTowards(Vector2 target)
		{
			Move(target - (Vector2)transform.position);
		}

		public void SetDashDirection(Vector2 dir)
		{
			_dashDir = dir.normalized;
		}

		public void SetFacingDirection(float x)
		{
			if (x > 0f)
			{
				FacingDirection = 1f;
				transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else if (x < 0f)
			{
				FacingDirection = -1f;
				transform.localScale = new Vector3(-1f, 1f, 1f);
			}
		}

		public void AddVelocity(Vector2 velocity)
		{
			_velocitySum += velocity;
		}

		public void Dash()
		{
			if (!IsDashing)
			{
				IsDashing = true;

				_dashEndTime = Time.time + DashDuration;
				OnDashStart?.Invoke();
			}
		}

		public void StopDash()
		{
			if (IsDashing)
			{
				IsDashing = false;
				OnDashEnd?.Invoke();

				_overlappedDestructables.Clear();
			}
		}

		public void ResetMoveSpeed()
		{
			MoveSpeed = _defaultMoveSpeed;
		}

		public void SetMoveDirection(Vector2 dir)
		{
			MoveDirection = dir.normalized;
		}
	}
}
