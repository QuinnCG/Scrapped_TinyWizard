using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI.Enemies
{
	public class MagmaSlime : Enemy
	{
		[SerializeField, Required]
		private GameObject BabyPrefab;

		private Tween _jump;

		protected override void Update()
		{
			base.Update();

			if (_jump == null || !_jump.IsActive())
			{
				_jump = Jump();
			}
		}

		private Tween Jump()
		{
			Vector2 start = Position;
			Vector2 end = start + (Vector2.right * 2f);
			float height = 0.5f;
			float duration = 0.65f;

			//Vector2 middle = Vector2.Lerp(start, end, 0.5f);
			//middle += Vector2.up * height;

			//var sequence = DOTween.Sequence();
			//sequence.Append(transform.DOMove(middle, duration / 2f).SetEase(Ease.OutSine));
			//sequence.Append(transform.DOMove(end, duration / 2f).SetEase(Ease.InSine));
			//sequence.AppendInterval(0.5f);

			float y = 0f;
			DOTween.To(() => y, x => y = x, height, duration / 2f).SetDelay(0.5f).SetEase(Ease.OutQuad).onComplete += () =>
			{
				DOTween.To(() => y, x => y = x, 0f, duration / 2f).SetEase(Ease.InQuad);
			};

			return DOTween.To(() => Position, x => transform.position = x + (Vector2.up * y), end, duration).SetDelay(0.5f);
		}
	}
}
