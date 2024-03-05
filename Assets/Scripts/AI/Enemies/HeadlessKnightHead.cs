using DG.Tweening;
using Quinn.Player;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.AI.Enemies
{
	public class HeadlessKnightHead : MonoBehaviour
	{
		[SerializeField]
		private float Duration = 0.9f;

		public Transform Origin { get; set; }

		private void Start()
		{
			Vector2 target = PlayerController.Instance.transform.position;
			transform.DOMove(target, Duration / 2f).onComplete += () =>
			{
				transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f);

				transform.DOMove(Origin.position, Duration / 2f).SetEase(Ease.InQuad).onComplete += () =>
				{
					var vfx = GetComponentInChildren<VisualEffect>();
					vfx.transform.parent = null;
					vfx.DestroyOnFinish();

					Destroy(gameObject);
				};
			};
		}
	}
}
