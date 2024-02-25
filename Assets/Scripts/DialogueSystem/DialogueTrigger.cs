using Quinn.Player;
using UnityEngine;

namespace Quinn.DialogueSystem
{
	[RequireComponent(typeof(Collider2D))]
	public class DialogueTrigger : MonoBehaviour
	{
		[SerializeField]
		private Dialogue Dialogue;

		private bool _inDialogue;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (_inDialogue) return;

			if (collision.gameObject == PlayerController.Instance.gameObject)
			{
				_inDialogue = true;

				DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
				DialogueManager.Instance.Display(Dialogue);
			}
		}

		private void OnDialogueEnd(Dialogue dialogue)
		{
			if (dialogue == Dialogue)
			{
				_inDialogue = false;
				DialogueManager.Instance.OnDialogueEnd -= OnDialogueEnd;
			}
		}
	}
}
