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

		public void ShowBossBar(Enemy enemy)
		{	
			_bossHealth = enemy.GetComponent<Health>();
			BossTitle.text = enemy.BossBarTitle;
			BossContainer.SetActive(true);

			enemy.GetComponent<Health>().OnDeath += () => HideBossBar();
		}

		public void HideBossBar()
		{
			_bossHealth = null;
			BossTitle.text = "Boss Title";
			BossContainer.SetActive(false);
		}
	}
}
