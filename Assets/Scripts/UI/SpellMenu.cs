using DG.Tweening;
using Quinn.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.UI
{
	public class SpellMenu : MonoBehaviour
	{
		[SerializeField, Required]
		private GameObject SpellSlot;

		[SerializeField]
		private float SlotDistance = 300f;

		[SerializeField]
		private float SelectionMaxDelta = 2f;

		[SerializeField]
		private float SelectedSlotScale = 1.3f;

		[SerializeField]
		private float SlotSelectDuration = 0.2f, SlotDeselectDuration = 0.2f;

		private RectTransform _rect;
		private RectTransform[] _children;

		private RectTransform _selectedSlot;
		private Vector2 _mouseDelta;

		private void Awake()
		{
			_rect = GetComponent<RectTransform>();
		}

		private void OnEnable()
		{
			if (transform.childCount > 0)
			{
				Destroy(transform.GetChild(0).gameObject);
			}

			var parent = new GameObject("Slots").transform;
			parent.parent = transform;

			foreach (var spell in Inventory.Instance.EquippedSpells)
			{
				var instance = Instantiate(SpellSlot, parent);
				var slot = instance.GetComponent<SpellSlot>();
				slot.SetSpell(spell);
			}

			UpdateLayout();
		}

		private void Update()
		{
			Vector2 delta = Input.mousePositionDelta;
			_mouseDelta += delta;
			_mouseDelta = _mouseDelta.normalized * Mathf.Min(_mouseDelta.magnitude, SelectionMaxDelta);

			Vector2 dir = _mouseDelta.normalized;
			Vector2 selection = _rect.anchoredPosition + (dir * SlotDistance);

			RectTransform closestSlot = GetNearestSlot(selection);
			if (closestSlot)
			{
				SelectSlot(closestSlot);
			}
		}

		private void OnDisable()
		{
			if (_selectedSlot)
			{
				for (int i = 0; i < _children.Length; i++)
				{
					if (_children[i] == _selectedSlot)
					{
						Inventory.Instance.SelectSpell(i);
						break;
					}
				}
			}
		}

		private void UpdateLayout()
		{
			var parent = transform.GetChild(0);

			_children = new RectTransform[parent.childCount];
			for (int i = 0; i < _children.Length; i++)
			{
				_children[i] = parent.GetChild(i).GetComponent<RectTransform>();
			}

			int count = _children.Length;
			float delta = Mathf.PI * 2f / count;
			float offset = (_children.Length % 2) == 0 ? (delta / 2f) : (delta / 4f);

			for (int i = 0; i < count; i++)
			{
				float angle = delta * i;
				angle += offset + delta;

				var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
				dir.Normalize();

				Vector3 pos = dir * SlotDistance;
				_children[i].anchoredPosition = pos;
			}
		}

		private RectTransform GetNearestSlot(Vector2 pos)
		{
			float bestDst = float.PositiveInfinity;
			RectTransform bestSlot = null;

			foreach (var child in _children)
			{
				float dst = Vector2.Distance(pos, child.position);

				if (dst < bestDst)
				{
					bestDst = dst;
					bestSlot = child;
				}
			}

			return bestSlot;
		}

		private void SelectSlot(RectTransform rect)
		{
			if (_selectedSlot == rect) return;

			foreach (var child in _children)
			{
				child.DOKill(true);
				child.DOScale(1f, SlotDeselectDuration).SetEase(Ease.OutBack);

				child.GetComponent<SpellSlot>().Select();
			}

			rect.DOKill(true);
			rect.DOScale(SelectedSlotScale, SlotSelectDuration).SetEase(Ease.OutBack);
			rect.GetComponent<SpellSlot>().Select();

			_selectedSlot = rect;
		}
	}
}
