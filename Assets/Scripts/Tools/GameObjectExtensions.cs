using UnityEngine;

namespace Quinn
{
	public static class GameObjectExtensions
	{
		public static bool IsLayer(this GameObject obj, string name)
		{
			return obj.layer == LayerMask.NameToLayer(name);
		}
	}
}
