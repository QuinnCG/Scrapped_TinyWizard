using System;
using System.Collections;
using UnityEngine;

namespace Quinn
{
	public class DestroyOnCondition : MonoBehaviour
	{
		private Func<bool> _condition;

		public IEnumerator Start()
		{
			yield return new WaitUntil(() => _condition());
			Destroy(gameObject);
		}

		public static void Create(Transform parent, Func<bool> condition)
		{
			parent.gameObject.AddComponent<DestroyOnCondition>()._condition = condition;
		}
	}
}
