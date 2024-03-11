using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class SaveManager : MonoBehaviour
	{
		public static SaveManager Instance { get; private set; }

		private static readonly Dictionary<string, object> _data = new();

		private void Awake()
		{
			Instance = this;

#if UNITY_EDITOR
			_data.Clear();
#endif
		}

		public static void Save<T>(string id, T data)
		{
			if (Has(id))
			{
				_data[id] = data;
			}
			else
			{
				_data.Add(id, data);
			}
		}
		public static void Save(string id)
		{
			if (!Has(id))
			{
				_data.Add(id, null);
			}
		}

		public static T Get<T>(string id)
		{
			return (T)_data[id];
		}

		public static void Delete(string id)
		{
			_data.Remove(id);
		}

		public static bool Has(string id)
		{
			return _data.ContainsKey(id);
		}
	}
}
