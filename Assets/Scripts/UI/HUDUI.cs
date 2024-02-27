using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class HUDUI : MonoBehaviour
	{
		public static HUDUI Instance { get; private set; }

		[SerializeField, Required, BoxGroup("Health")]
		private Slider HealthBar;

		[SerializeField, Required, BoxGroup("Health")]
		private Health PlayerHealth;

		[SerializeField, BoxGroup("Health")]
		private float PlayerHealthToWidthRatio = 1f;

		[SerializeField, Required, BoxGroup("Boss")]
		private GameObject BossContainer;

		[SerializeField, Required, BoxGroup("Boss")]
		private TextMeshProUGUI BossTitle;

		[SerializeField, Required, BoxGroup("Boss")]
		private Slider BossBar;

		private Health _bossHealth;
		private RectTransform _playerHealthBarRect;

		private void Awake()
		{
			Instance = this;
			BossContainer.SetActive(false);

			_playerHealthBarRect = HealthBar.GetComponent<RectTransform>();
			PlayerHealth.OnMaxChange += OnMaxChange;
		}

		private void Update()
		{
			HealthBar.value = PlayerHealth.Percent;

			if (_bossHealth)
			{
				BossBar.value = _bossHealth.Percent;
			}
		}

		public void ShowBossBar(string title, Health health)
		{
			_bossHealth = health;
			BossTitle.text = title;
			BossContainer.SetActive(true);

			health.OnDeath += () => HideBossBar();
		}

		public void HideBossBar()
		{
			_bossHealth = null;
			BossTitle.text = "Boss Title";
			BossContainer.SetActive(false);
		}

		private void OnMaxChange(float max)
		{
			var size = _playerHealthBarRect.sizeDelta;
			size.x = max * PlayerHealthToWidthRatio;

			_playerHealthBarRect.sizeDelta = size;
		}
	}
}
