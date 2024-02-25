using System;
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

		public bool IsMoving { get; private set; }
		public bool IsDashing { get; private set; }

		public float FacingDirection { get; private set; } = 1f;
		public Vector2 Velocity => _rb.velocity;

		public event Action OnDashStart, OnDashEnd;

		private Rigidbody2D _rb;

		private Vector2 _velocitySum;
		private Vector2 _dashDir = Vector2.right;
		private bool _wasMovingLastFrame;
		private float _dashEndTime;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			if (IsDashing)
			{
				_velocitySum += DashSpeed * _dashDir;

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

		public void Move(Vector2 dir)
		{
			if (!IsDashing)
			{
				dir.Normalize();

				_velocitySum += dir * MoveSpeed;
				_wasMovingLastFrame = dir.sqrMagnitude > 0f;

				if (dir.sqrMagnitude > 0f)
				{
					SetDashDirection(dir);
					SetFacingDirection(Mathf.Sign(dir.x));
				}
			}
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

		public void MoveTowards(Vector2 target)
		{
			Move(target - (Vector2)transform.position);
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
			}
		}
	}
}
