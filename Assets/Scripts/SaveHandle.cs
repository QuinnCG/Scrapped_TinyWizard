using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn
{
	[ExecuteAlways]
	public class SaveHandle : MonoBehaviour
	{
		[SerializeField,]
		private Guid GUID = Guid.Empty;

		[SerializeField, ReadOnly]
		private string ID;

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

		private void Update()
		{
			if (Application.isPlaying)
			{
				SaveManager.Save(GUID, "my_key", 3);
			}
		}
	}
}
