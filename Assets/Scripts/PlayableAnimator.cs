using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Quinn
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class PlayableAnimator : MonoBehaviour
	{
		[SerializeField]
		private PlayableAnimatorType AnimatorType = PlayableAnimatorType.Manual;

		[Space, SerializeField, ShowIf(nameof(AnimatorType), Value = PlayableAnimatorType.PlayDefault)]
		private AnimationClip DefaultAnimation;

		[Space, SerializeField, ShowIf(nameof(AnimatorType), Value = PlayableAnimatorType.PlaySequence)]
		private AnimationClip[] AnimationSequence;

		public float Speed
		{
			get => _speed;
			set
			{
				_speed = value;
				_graph.GetRootPlayable(0).SetSpeed((float)value);
			}
		}

		private float _speed = 1f;

		private PlayableGraph _graph;
		private AnimationPlayableOutput _output;

		private AnimationClip _activeAnim;
		private AnimationClipPlayable _activeClip;

		private void Awake()
		{
			if (!TryGetComponent(out Animator animator))
			{
				animator = gameObject.AddComponent<Animator>();
			}

			_graph = PlayableGraph.Create("Animation Graph");
			_graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
			_output = AnimationPlayableOutput.Create(_graph, "Animation Output", animator);

			if (AnimatorType == PlayableAnimatorType.PlayDefault)
			{
				Play(DefaultAnimation);
			}
		}

		private IEnumerator Start()
		{
			if (AnimatorType == PlayableAnimatorType.PlaySequence)
			{
				for (int i = 0; i < AnimationSequence.Length; i++)
				{
					var anim = AnimationSequence[i];

					Play(anim);
					yield return new WaitForSeconds(anim.length - float.Epsilon);
				}
			}
		}

		private void OnDestroy()
		{
			_graph.Destroy();
		}

		public AnimationClip Play(AnimationClip anim)
		{
			if (!enabled)
				return null;

			StopAllCoroutines();

			if (anim == _activeAnim)
				return _activeAnim;

			if (_activeClip.IsValid())
				_activeClip.Destroy();

			var clip = AnimationClipPlayable.Create(_graph, anim);
			_output.SetSourcePlayable(clip);

			if (!_graph.IsPlaying())
			{
				_graph.Play();
			}

			_activeAnim = anim;
			_activeClip = clip;

			return _activeAnim;
		}

		public void Stop()
		{
			_graph.Stop();
		}
	}
}
