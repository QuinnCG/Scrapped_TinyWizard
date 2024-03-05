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

		[SerializeField]
		private float MinDistance = 2f;

		[SerializeField]
		private float DistanceFactor = 1.5f;

		public Transform Origin { get; set; }

		private void Start()
		{
			float halfDur = Duration / 2f;

			Vector2 target = PlayerController.Instance.transform.position;
			Vector2 diff = target - (Vector2)transform.position;

			target = (Vector2)transform.position + (diff.normalized * Mathf.Max(MinDistance, diff.magnitude * DistanceFactor));

			transform.DOMove(target, halfDur).onComplete += () =>
			{
				transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f);

				transform.DOMove(Origin.position, halfDur).SetEase(Ease.InQuad).onComplete += () =>
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
