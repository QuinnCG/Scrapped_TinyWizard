using UnityEngine;

namespace Quinn.DialogueSystem
{
	[System.Serializable]
	public class Dialogue
	{
		public string Speaker = "Speak Name";

		[Multiline]
		public string[] Messages;
	}
}
