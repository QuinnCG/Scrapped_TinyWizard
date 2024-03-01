using Cinemachine;
using DG.Tweening;
using Quinn.Player;
using Quinn.RoomSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class CameraManager : MonoBehaviour
	{
		public static CameraManager Instance { get; private set; }

		[SerializeField, Required, BoxGroup("Fade To Black")]
		private CanvasGroup Blackout;

		[field: SerializeField, BoxGroup("Fade To Black")]
		public float FadeToBlackDuration { get; private set; } = 0.5f;

		[field: SerializeField, BoxGroup("Fade To Black")]
		public float FadeFromBlackDuration { get; private set; } = 1f;

		private CinemachineVirtualCamera _activeVCam;
		private Tween _fadeTween;

		private void Awake()
		{
			Instance = this;
		}

		public void SetVirtualCamera(CinemachineVirtualCamera virtualCamera)
		{
			if (_activeVCam != null)
			{
				_activeVCam.enabled = false;
			}

			_activeVCam = virtualCamera;

			virtualCamera.Follow = PlayerController.Instance.GetCameraTarget();
			virtualCamera.enabled = true;

		}

		public float FadeToBlack()
		{
			StopFade();
			_fadeTween = DOTween.To(() => Blackout.alpha, x => Blackout.alpha = x, 1f, FadeToBlackDuration);

			return Time.time + FadeToBlackDuration;
		}

		public float FadeFromBlack()
		{
			StopFade();
			_fadeTween = DOTween.To(() => Blackout.alpha, x => Blackout.alpha = x, 0f, FadeFromBlackDuration);

			return Time.time + FadeFromBlackDuration;
		}

		private void StopFade()
		{
			if (_fadeTween != null && _fadeTween.IsActive())
			{
				_fadeTween.Kill();
			}
		}
	}
}
