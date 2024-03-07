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

		public Vector2 Target { get; set; }
		public Transform Origin { get; set; }

		private void Start()
		{
			float halfDur = Duration / 2f;

			Vector2 diff = Target - (Vector2)transform.position;
			Target = (Vector2)transform.position + (diff.normalized * Mathf.Max(MinDistance, diff.magnitude * DistanceFactor));

			transform.DOMove(Target, halfDur).onComplete += () =>
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
