using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Damage))]
	public class Knockback : MonoBehaviour
	{
		// A generalized base damage for the game. Based on 5 hits to kill a 100 HP enemy.
		public const float BaseSpellDamage = 20f;

		// When the knockback velocity reaches this threshold, cancel the knockback.
		public const float VelocityEndTheshold = 0.5f;

		[SerializeField]
		private float BaseSpeed = 8f;

		[SerializeField]
		private float DecayRate = 16f;

		[Space, SerializeField]
		private bool UseCustomMass;

		[SerializeField, ShowIf(nameof(UseCustomMass))]
		private float Mass = 1f;

		private Movement _movement;

		private float _mass;
		private bool _shouldKnockback;
		private Vector2 _knockbacDir;
		private float _knockbackSpeed;

		private void Awake()
		{
			_movement = GetComponent<Movement>();
			GetComponent<Damage>().OnDamaged += OnDamaged;

			var rb = GetComponent<Rigidbody2D>();
			_mass = UseCustomMass ? Mass : rb.mass;
		}

		private void Update()
		{
			if (_shouldKnockback)
			{
				if (_knockbackSpeed <= VelocityEndTheshold)
				{
					_shouldKnockback = false;
					return;
				}

				_movement.AddVelocity(_knockbacDir * _knockbackSpeed);
				_knockbackSpeed -= DecayRate * Time.deltaTime;
			}
		}

		private void OnDamaged(DamageInfo info, DamageEfficiencyType type)
		{
			if (type != DamageEfficiencyType.Resistant)
			{
				_shouldKnockback = true;

				_knockbacDir = info.Direction.normalized;
				_knockbackSpeed = BaseSpeed / _mass;
			}
		}
	}
}
