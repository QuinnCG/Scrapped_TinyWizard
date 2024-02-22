using System;
using UnityEngine;

namespace Quinn.SpellSystem
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class Missile : MonoBehaviour
	{
		public event Action<GameObject> OnHit;

		private Rigidbody2D _rb;

		private Vector2 _dir;
		private MissileInfo _info;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			_rb.velocity = _dir * _info.Speed;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			OnHit?.Invoke(collision.gameObject);
		}

		public void Launch(Vector2 dir, MissileInfo info)
		{
			_dir = dir.normalized;
			_info = info;
		}
	}
}
