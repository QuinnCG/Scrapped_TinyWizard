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

		public event Action<GameObject> OnHit;
		public event Action OnDestroyed;

		public Rigidbody2D Rigidbody { get; private set; }
		public Vector2 Velocity => Rigidbody.velocity;

		private Vector2 _dir;
		private MissileInfo _info;

		private void Awake()
		{
			Rigidbody = GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			Rigidbody.velocity = _dir * _info.Speed;
		}

		protected virtual void OnTriggerEnter2D(Collider2D collision)
		{
			OnHit?.Invoke(collision.gameObject);
		}

		protected virtual void OnDestroy()
		{
			OnDestroyed?.Invoke();

			if (Attached)
			{
				Attached.transform.parent = null;
			}
		}

		public void Launch(Vector2 dir, MissileInfo info)
		{
			_dir = dir.normalized;
			_info = info;

			GetComponent<CircleCollider2D>().radius = info.HitRadius;

			if (Lifespan > 0f)
			{
				Destroy(gameObject, Lifespan);
			}
		}
	}
}
