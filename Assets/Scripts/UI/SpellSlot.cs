using DG.Tweening;
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

		[SerializeField]
		private float SelectedSlotScale = 1.2f;

		[SerializeField]
		private float SlotSelectDuration = 0.2f, SlotDeselectDuration = 0.2f;

		private RectTransform _rect;

		private void Awake()
		{
			_rect = GetComponent<RectTransform>();
		}

		public void SetSpell(SpellItem spell)
		{
			Icon.sprite = spell.Icon;
		}

		public void Select()
		{
			Outline.SetActive(true);

			_rect.DOKill(true);
			_rect.DOScale(SelectedSlotScale, SlotSelectDuration).SetEase(Ease.OutBack);
		}

		public void Deselect()
		{
			Outline.SetActive(false);

			_rect.DOKill(true);
			_rect.DOScale(1f, SlotDeselectDuration).SetEase(Ease.OutBack);
		}
	}
}
