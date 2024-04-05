using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Quinn.Cinematics
{
	public class OpeningCinematic : MonoBehaviour
	{
		[SerializeField]
		private GameObject Portal;

		private IEnumerator Start()
		{
			Portal.SetActive(false);
			yield return new WaitForSeconds(5f);

			Portal.SetActive(true);
			Portal.transform.DOScale(0.95f, 0.3f)
				.SetEase(Ease.InOutCirc)
				.SetLoops(-1, LoopType.Yoyo);

			yield break;
		}
	}
}
