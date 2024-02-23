using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Health))]
	public class Enemy : MonoBehaviour
	{
		[SerializeField]
		private bool DisplayBossBar;

		[field: SerializeField, ShowIf(nameof(DisplayBossBar))]
		public string BossBarTitle { get; private set; } = "Enemy Title";
	}
}
