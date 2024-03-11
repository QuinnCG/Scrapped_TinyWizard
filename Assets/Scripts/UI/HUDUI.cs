using Quinn.DamageSystem;
using Quinn.Player;
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

		[SerializeField, Required]
		private GameObject PauseMenu;

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

		[SerializeField, Required, BoxGroup("Inventory")]
		private GameObject SpellMenu;

		[SerializeField, Required, BoxGroup("Inventory")]
		private GameObject ItemPickupNotification;

		[SerializeField, Required, BoxGroup("Inventory")]
		private GameObject ItemConsumedNotification;

		private Health _bossHealth;
		private RectTransform _playerHealthBarRect;

		private bool _isSpellMenuOpen;
		private InputReader.InputDisablerHandle _spellMenuInputHandle;

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

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				// TODO: Toggle pause menu.
			}

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				if (!_isSpellMenuOpen)
				{
					_isSpellMenuOpen = true;
					SpellMenu.SetActive(true);
					_spellMenuInputHandle = InputReader.Instance.DisableInput(true);
					Crosshair.Instance.enabled = false;

					Cursor.lockState = CursorLockMode.Locked;
				}
			}
			else if (Input.GetKeyUp(KeyCode.Tab))
			{
				if (_isSpellMenuOpen)
				{
					_isSpellMenuOpen = false;
					SpellMenu.SetActive(false);
					InputReader.Instance.EnableInput(_spellMenuInputHandle);
					Crosshair.Instance.enabled = true;

					Cursor.lockState = CursorLockMode.Confined;
				}
			}
		}

		public static void ShowCursor()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;
		}

		public static void HideCursor()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;
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

		public void DisplayItemPickedup(Item item, int count = 1)
		{
			Debug.Log($"<b>Item has been picked up: {item.Name}!</b>");
		}

		public void DisplayItemConsumed(Item item)
		{
			Debug.Log($"<b>Item has been consumed: {item.Name}!</b>");
		}

		private void OnMaxChange(float max)
		{
			var size = _playerHealthBarRect.sizeDelta;
			size.x = max * PlayerHealthToWidthRatio;

			_playerHealthBarRect.sizeDelta = size;
		}
	}
}
