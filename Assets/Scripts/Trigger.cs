using Quinn.AI;
using Quinn.Player;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Quinn
{
	[RequireComponent(typeof(Collider2D))]
	public class Trigger : MonoBehaviour
	{
		public enum TriggerType
		{
			Player,
			Enemy,
			Both
		}

		[SerializeField]
		private TriggerType Type = TriggerType.Player;

		[SerializeField]
		private bool CanRetrigger = false;

		[SerializeField]
		private UnityEvent OnTriggerEvent;

		public event Action OnTrigger;

		private bool _canTrigger = true;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!_canTrigger) return;

			GameObject hit = collision.gameObject;
			GameObject player = PlayerController.Instance.gameObject;

			switch (Type)
			{
				case TriggerType.Player:
				if (hit == player) Execute();
				break;

				case TriggerType.Enemy:
				if (hit.TryGetComponent(out Enemy _)) Execute();
				break;

				case TriggerType.Both:
				if (hit == player
				|| hit.TryGetComponent(out Enemy _))
					Execute();
				break;
			}
		}

		private void Execute()
		{
			OnTrigger?.Invoke();
			OnTriggerEvent?.Invoke();

			_canTrigger = CanRetrigger;

			if (TryGetComponent(out SaveHandle handle))
			{
				SaveManager.Save(handle.GUID);
				Debug.Log("Saving!");
			}
		}
	}
}
