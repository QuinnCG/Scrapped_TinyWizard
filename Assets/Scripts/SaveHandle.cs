using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn
{
	[ExecuteAlways]
	public class SaveHandle : MonoBehaviour
	{
		[field: SerializeField]
		public Guid GUID { get; private set; } = Guid.Empty;

		[SerializeField, ReadOnly]
		private string ID;

		[SerializeField]
		private bool DestroyIfKeyFound;

		[Button]
		public void Regenerate()
		{
			GUID = Guid.NewGuid();
			ID = GUID.ToString();
		}

		[Button("Reset")]
		public void ResetGUID()
		{
			GUID = Guid.Empty;
			ID = "No ID";
		}

		private void Awake()
		{
			if (DestroyIfKeyFound)
			{
				Debug.Log($"Found key: {SaveManager.Contains(GUID, ID)}!");
			}
			
			if (DestroyIfKeyFound && SaveManager.Contains(GUID, ID))
			{
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			if (GUID == Guid.Empty)
			{
				Regenerate();
			}
			else
			{
				ID = GUID.ToString();
				if (GUID == Guid.Empty)
				{
					ResetGUID();
				}
			}
		}
	}
}
