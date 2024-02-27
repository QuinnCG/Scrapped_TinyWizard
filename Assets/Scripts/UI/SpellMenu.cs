using UnityEngine;

namespace Quinn.UI
{
	public class SpellMenu : MonoBehaviour
	{
		[SerializeField]
		private float SlotDistance = 300f;

		private RectTransform[] Children;

		private void Start()
		{
			Children = new RectTransform[transform.childCount];
			for (int i = 0; i < Children.Length; i++)
			{
				Children[i] = transform.GetChild(i).GetComponent<RectTransform>();
			}

			UpdateLayout();
		}

		private void UpdateLayout()
		{
			int count = Children.Length;
			float delta = Mathf.PI * 2f / count;
			float offset = (Children.Length % 2) == 0 ? (delta / 2f) : (delta / 4f);

			for (int i = 0; i < count; i++)
			{
				float angle = delta * i;
				angle += offset + delta;

				var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
				dir.Normalize();

				Vector3 pos = dir * SlotDistance;
				Children[i].anchoredPosition = pos;
			}
		}
	}
}
