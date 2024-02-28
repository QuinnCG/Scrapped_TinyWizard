using Quinn.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class SpellSlot : MonoBehaviour
	{
		[SerializeField, Required]
		private Image Icon;

		[SerializeField, Required]
		private GameObject Outline;

		public void SetSpell(SpellItem spell)
		{
			Icon.sprite = spell.Icon;
		}

		public void Select()
		{
			Outline.SetActive(true);
		}

		public void Deselect()
		{
			Outline.SetActive(false);
		}
	}
}
