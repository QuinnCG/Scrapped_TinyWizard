using System;
using UnityEngine;

namespace Quinn.SpellSystem
{
	[RequireComponent(typeof(CircleCollider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class Missile : MonoBehaviour
	{
		public GameObject Attached { get; set; }
		public float Lifespan { get; set; } = 10f;
		public MissileInfo Info { get; private set; }

		public event Action<GameObject> OnHit;
		public event Action OnDestroyed;

		public Rigidbody2D Rigidbody { get; private set; }
		public Vector2 Velocity => Rigidbody.linearVelocity;

		private Vector2 _dir;

		private void Awake()
		{
			Rigidbody = GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			Rigidbody.linearVelocity = _dir * Info.Speed;
		}

		protected virtual void OnTriggerEnter2D(Collider2D collision)
		{
			OnHit?.Invoke(collision.gameObject);
		}

		protected virtual void OnDestroy()
		{
			OnDestroyed?.Invoke();
		}

		public void Launch(Vector2 dir, MissileInfo info)
		{
			_dir = dir.normalized;
			Info = info;

			GetComponent<CircleCollider2D>().radius = info.HitRadius;

			if (Lifespan > 0f)
			{
				Destroy(gameObject, Lifespan);
			}
		}
	}
}
