using Quinn.AI;
using Sirenix.OdinInspector;
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

		[SerializeField, Required, BoxGroup("Boss")]
		private GameObject BossContainer;

		[SerializeField, Required, BoxGroup("Boss")]
		private TextMeshProUGUI BossTitle;

		[SerializeField, Required, BoxGroup("Boss")]
		private Slider BossBar;

		private Health _bossHealth;

		private void Awake()
		{
			Instance = this;
			BossContainer.SetActive(false);
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
	}
}
