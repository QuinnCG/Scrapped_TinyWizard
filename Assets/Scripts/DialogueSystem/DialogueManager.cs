using Quinn.Player;
using Sirenix.OdinInspector;
using System;
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

		public event Action<Dialogue> OnDialogueEnd;

		public bool InDialogue { get; private set; }

		private InputReader.InputDisablerHandle _inputDisabler;
		private int _index;
		private Dialogue _dialogue;
		private bool _isWriting;

		private void Awake()
		{
			Instance = this;
			Container.SetActive(false);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space)
				|| Input.GetKeyDown(KeyCode.E)
				|| Input.GetKeyDown(KeyCode.Escape)
				|| Input.GetMouseButtonDown(0))
			{
				if (_isWriting)
				{
					StopAllCoroutines();
					Dialogue.text = _dialogue.Messages[_index];
					_isWriting = false;
				}
				else
				{
					Next();
				}
			}
		}

		public WaitUntil Display(Dialogue dialogue)
		{
			_inputDisabler = InputReader.Instance.DisableInput();

			Container.SetActive(true);
			_index = -1;
			_dialogue = dialogue;

			InDialogue = true;

			Speaker.text = dialogue.Speaker;
			Next();

			return new WaitUntil(() => !InDialogue);
		}

		private void Next()
		{
			_index++;

			if (_index >= _dialogue.Messages.Length)
			{
				OnDialogueEnd?.Invoke(_dialogue);

				Hide();
				_dialogue = null;
				_index = 0;

				return;
			}

			StopAllCoroutines();
			StartCoroutine(WriteSequence());
		}

		private void Hide()
		{
			InputReader.Instance.EnableInput(_inputDisabler);

			Container.SetActive(false);
			StopAllCoroutines();

			InDialogue = false;
		}

		private IEnumerator WriteSequence()
		{
			var builder = new StringBuilder();
			_isWriting = true;

			int count = _dialogue.Messages[_index].Length;
			for (int i = 0; i < count; i++)
			{
				builder.Append(_dialogue.Messages[_index][i]);
				Dialogue.text = builder.ToString();

				yield return new WaitForSeconds(WriteInterval);
			}

			_isWriting = false;
		}
	}
}
