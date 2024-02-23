using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class HUDUI : MonoBehaviour
	{
		[SerializeField, Required, BoxGroup("Health")]
		private Slider HealthBar;

		[SerializeField, Required, BoxGroup("Health")]
		private Health PlayerHealth;

		private void Update()
		{
			HealthBar.value = PlayerHealth.Percent;
		}
	}
}
