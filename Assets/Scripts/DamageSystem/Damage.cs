using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

namespace Quinn.DamageSystem
{
	public class Damage : MonoBehaviour
	{
		[field: SerializeField]
		public Team Team { get; private set; }

		[SerializeField]
		private float ImmunityDuration = -1f;

		[SerializeField, BoxGroup("Elemental")]
		private ElementType Resistances;

		[SerializeField, BoxGroup("Elemental")]
		private ElementType Weaknesses;

		[Space, SerializeField, BoxGroup("Elemental"), HideIf(nameof(Resistances), (ElementType)0)]
		private float ResistanceDamageFactor = 0.75f;

		[SerializeField, BoxGroup("Elemental"), HideIf(nameof(Weaknesses), (ElementType)0)]
		private float WeaknessDamageFactor = 1.5f;

		[SerializeField, BoxGroup("VFX")]
		private bool FlashWhite;

		[SerializeField, BoxGroup("VFX")]
		private bool BlinkInAndOut;

		public event Action<DamageInfo, DamageEfficiencyType> OnDamaged;

		private SpriteRenderer[] _renderers;

		private Health _health;
		private float _nextCanDamageTime;

		private void Awake()
		{
			_renderers = GetComponentsInChildren<SpriteRenderer>();
			TryGetComponent(out _health);
		}

		public bool CanTakeDamage(Team source)
		{
			if (_health && _health.IsDead)
			{
				return false;
			}

			return Team != source && Time.time > _nextCanDamageTime;
		}
		public bool CanTakeDamage(Damage source)
		{
			return CanTakeDamage(source.Team);
		}

		public bool TakeDamage(DamageInfo info)
		{
			if (!CanTakeDamage(info.Source.Team))
			{
				return false;
			}

			_nextCanDamageTime = Time.time + ImmunityDuration;

			var efficiencyType = ModifyDamage(info);
			OnDamaged?.Invoke(info, efficiencyType);

			if (FlashWhite)
			{
				ResetVFX();
				StartCoroutine(FlashWhiteSequence());
			}

			if (BlinkInAndOut)
			{
				ResetVFX();
				StartCoroutine(BlinkInAndOutSequence());
			}

			return true;
		}

		private DamageEfficiencyType ModifyDamage(DamageInfo info)
		{
			float modifier = 1f;
			var efficiencyType = DamageEfficiencyType.Normal;

			if ((info.Element & Resistances) > 0)
			{
				modifier += ResistanceDamageFactor;
				efficiencyType = DamageEfficiencyType.Resistant;
			}
			else if ((info.Element & Weaknesses) > 0)
			{
				modifier += WeaknessDamageFactor;
				efficiencyType = DamageEfficiencyType.Weak;
			}

			info.Damage *= modifier;
			return efficiencyType;
		}

		private void ResetVFX()
		{
			StopAllCoroutines();

			foreach (var renderer in _renderers)
			{
				renderer.material.SetFloat("_Hurt", 0f);

				var color = renderer.color;
				color.a = 1f;
				renderer.color = color;
			}
		}

		private IEnumerator FlashWhiteSequence()
		{
			const float FlashInRate = 1f / 0.05f;
			const float FlashOutRate = 1f / 0.1f;

			while (_renderers.Any(x => x.material.GetFloat("_Hurt") < 1f))
			{
				foreach (var renderer in _renderers)
				{
					float hurt = renderer.material.GetFloat("_Hurt");
					renderer.material.SetFloat("_Hurt", Mathf.Min(1f, hurt + (FlashInRate * Time.deltaTime)));
				}

				yield return null;
			}

			while (_renderers.Any(x => x.material.GetFloat("_Hurt") > 0f))
			{
				foreach (var renderer in _renderers)
				{
					float hurt = renderer.material.GetFloat("_Hurt");
					renderer.material.SetFloat("_Hurt", Mathf.Max(0f, hurt - (FlashOutRate * Time.deltaTime)));
				}

				yield return null;
			}
		}

		private IEnumerator BlinkInAndOutSequence()
		{
			const float FadeOutRate = 1f / 0.1f;
			const float FadeInRate = 1f / 0.1f;
			const float IntervalDuration = 0.15f;

			for (int i = 0; i < 4; i++)
			{
				while (_renderers.Any(x => x.color.a > 0f))
				{
					foreach (var renderer in _renderers)
					{
						var color = renderer.color;
						color.a = Mathf.Max(0f, color.a - (FadeOutRate * Time.deltaTime));
						renderer.color = color;
					}

					yield return null;
				}

				while (_renderers.Any(x => x.color.a < 1f))
				{
					foreach (var renderer in _renderers)
					{
						var color = renderer.color;
						color.a = Mathf.Min(1f, color.a + (FadeInRate * Time.deltaTime));
						renderer.color = color;
					}

					yield return null;
				}

				yield return new WaitForSeconds(IntervalDuration);
			}
		}
	}
}
