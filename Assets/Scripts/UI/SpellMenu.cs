using DG.Tweening;
using UnityEngine;

namespace Quinn.UI
{
	public class SpellMenu : MonoBehaviour
	{
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
		private Vector2 _delta;

		private void Start()
		{
			_rect = GetComponent<RectTransform>();

			_children = new RectTransform[transform.childCount];
			for (int i = 0; i < _children.Length; i++)
			{
				_children[i] = transform.GetChild(i).GetComponent<RectTransform>();
			}

			UpdateLayout();
		}

		private void Update()
		{
			Vector2 delta = Input.mousePositionDelta;
			_delta += delta;
			_delta = _delta.normalized * Mathf.Min(_delta.magnitude, SelectionMaxDelta);

			Vector2 dir = _delta.normalized;
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
				Debug.Log(_selectedSlot.gameObject.name);
			}
		}

		private void UpdateLayout()
		{
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
			}

			rect.DOKill(true);
			rect.DOScale(SelectedSlotScale, SlotSelectDuration).SetEase(Ease.OutBack);

			_selectedSlot = rect;
		}
	}
}
