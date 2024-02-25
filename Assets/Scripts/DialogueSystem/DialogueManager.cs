using Sirenix.OdinInspector;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace Quinn.DialogueSystem
{
	public class DialogueManager : MonoBehaviour
	{
		public static DialogueManager Instance { get; private set; }

		[SerializeField]
		private float WriteInterval = 0.05f;

		[SerializeField, Required, Space]
		private GameObject Container;

		[SerializeField, Required]
		private TextMeshProUGUI Speaker, Dialogue;

		private int _index;
		private Dialogue _dialogue;
		private bool _shouldSkip;

		private void Awake()
		{
			Instance = this;
			Container.SetActive(false);
		}

		private void Update()
		{
			if (Input.anyKeyDown)
			{
				_shouldSkip = true;
			}
		}

		public void Display(Dialogue dialogue)
		{
			Container.SetActive(true);
			_index = -1;
			_dialogue = dialogue;

			Speaker.text = dialogue.Speaker;
			Next();
		}

		public void Next()
		{
			_index++;

			if (_index >= _dialogue.Messages.Length)
			{
				Hide();
				_dialogue = null;
				_index = 0;
			}

			StopAllCoroutines();
			StartCoroutine(WriteSequence());
		}

		private void Hide()
		{
			Container.SetActive(false);
		}

		private IEnumerator WriteSequence()
		{
			var builder = new StringBuilder();

			int count = _dialogue.Messages[_index].Length;
			for (int i = 0; i < count; i++)
			{
				builder.Append(_dialogue.Messages[_index][i]);
				Dialogue.text = builder.ToString();

				if (_shouldSkip)
				{
					_shouldSkip = false;
					Dialogue.text = _dialogue.Messages[_index];

					yield break;
				}

				yield return new WaitForSeconds(WriteInterval);
			}
		}
	}
}
