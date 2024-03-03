using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn
{
	public class SaveManager : MonoBehaviour
	{
		[Serializable]
		public class SaveEntry
		{
			[ReadOnly]
			public Guid Guid;

			[HorizontalGroup, ReadOnly]
			public string Key;

			[HorizontalGroup, ReadOnly]
			public string Value;

			public SaveEntry() { }
			public SaveEntry(Guid key, string subkey, object value)
			{
				Guid = key;
				Key = subkey;
				Value = value?.ToString();
			}
		}

		private static SaveManager _instance;

		[SerializeField, ReadOnly]
		private List<SaveEntry> SaveData;

		private static readonly Dictionary<string, object> _entries = new();

		private void Awake()
		{
			_entries.Clear();
			SaveData.Clear();
			_instance = this;
		}

		public static bool Contains(Guid key, string subkey)
		{
			return _entries.ContainsKey(GenerateKey(key, subkey));
		}

		public static void Save<T>(Guid key, string subkey, T data)
		{
			string realKey = GenerateKey(key, subkey);

			if (_entries.TryGetValue(realKey, out var _))
			{
				_entries[realKey] = data;

#if UNITY_EDITOR
				var entry = _instance.SaveData.Where(x => x.Guid == key && x.Key == subkey).FirstOrDefault();
				if (entry != null)
				{
					entry.Value = data.ToString();
				}
#endif

				return;
			}

			_entries.Add(realKey, data);

#if UNITY_EDITOR
			_instance.SaveData.Add(new SaveEntry(key, subkey, data));
#endif
		}
		public static void Save<T>(Guid key, T data)
		{
			Save(key, string.Empty, data);
		}
		public static void Save(Guid key)
		{
			Save<object>(key, null);
		}

		public static void Delete(Guid key, string subkey)
		{
			if (Contains(key, subkey))
			{
				_entries.Remove(GenerateKey(key, subkey));

#if UNITY_EDITOR
				var entry = _instance.SaveData.Where(x => x.Guid == key && x.Key == subkey).FirstOrDefault();
				if (entry != null)
				{
					_instance.SaveData.Remove(entry);
				}
#endif
			}
		}

		public static T Load<T>(Guid key, string subkey)
		{
			Debug.Assert(Contains(key, subkey), $"SaveManager cannot find key '{GenerateKey(key, subkey)}'!");
			Debug.Assert(_entries[GenerateKey(key, subkey)] is T, $"SaveManager cannot convert key '{GenerateKey(key, subkey)}' into type '{typeof(T)}'!");

			return (T)_entries[GenerateKey(key, subkey)];
		}
		public static T Load<T>(Guid key)
		{
			return Load<T>(key, string.Empty);
		}

		private static string GenerateKey(Guid key, string subkey)
		{
			return $"{key}.{subkey}";
		}
	}
}
