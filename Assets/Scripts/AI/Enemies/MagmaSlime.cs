using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Quinn.AI.Enemies
{
	public class MagmaSlime : Enemy
	{
		[SerializeField, Required]
		private GameObject BabyPrefab;

		private IEnumerator _jump;

		protected override void Update()
		{
			base.Update();

			if (_jump == null)
			{
				_jump = Jump();
				StartCoroutine(_jump);
			}
		}

		private IEnumerator Jump()
		{
			Vector2 start = Position;
			Vector2 end = start + (Vector2.right * 2f);

			float height = 2f;
			float speed = 1f;

			float totalDst = Vector2.Distance(start, end);
			float duration = totalDst / speed;

			float xDir = end.x - start.x;
			Vector2 center = Vector2.Lerp(start, end, 0.5f);

			float progress = 0f;

			Debug.DrawLine(start, end, Color.white, float.PositiveInfinity);

			while (Mathf.Abs(progress) > 0.1f)
			{
				Debug.DrawLine(center, Position, Color.yellow, float.PositiveInfinity);

				Vector2 toPos = (Position - start).normalized;
				Vector2 toEnd = (end - start).normalized;

				float projDst = Vector2.Dot(toPos, toEnd) / toEnd.magnitude;
				progress = projDst / totalDst;

				Debug.Log(progress);

				float yVel = progress < 0.5f ? 1f : -1f;
				yVel *= height / (duration / 2f);

				Movement.AddVelocity(new Vector2(xDir * speed, yVel));

				yield return null;
			}
		}
	}
}
