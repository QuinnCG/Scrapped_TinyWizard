using Quinn.Player;
using Sirenix.OdinInspector;
using System.Collections.Generic;
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

		private RectTransform _rect;
		private readonly List<RectTransform> _children = new();

		private RectTransform _selectedSlot;
		private Vector2 _mouseDelta;

		private void Awake()
		{
			_rect = GetComponent<RectTransform>();
		}

		private void OnEnable()
		{
			while (_children.Count > 0)
			{
				Destroy(_children[0].gameObject);
				_children.RemoveAt(0);
			}

			foreach (var spell in Inventory.Instance.EquippedSpells)
			{
				var instance = Instantiate(SpellSlot, transform);
				var slot = instance.GetComponent<SpellSlot>();
				slot.SetSpell(spell);

				_children.Add(instance.GetComponent<RectTransform>());
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
				for (int i = 0; i < _children.Count; i++)
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
			int count = _children.Count;
			float delta = Mathf.PI * 2f / count;
			float offset = (_children.Count % 2) == 0 ? (delta / 2f) : (delta / 4f);

			for (int i = 0; i < count; i++)
			{
				float angle = delta * i;
				angle += offset + (delta * 3f);

				var dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
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
			_selectedSlot = rect;

			foreach (var child in _children)
			{
				child.GetComponent<SpellSlot>().Deselect();
			}

			rect.GetComponent<SpellSlot>().Select();
		}
	}
}
