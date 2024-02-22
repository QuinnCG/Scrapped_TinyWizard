namespace Quinn
{
	[System.Serializable]
	public class DamageInfo
	{
		public float Damage;
		public Damage Source;
		public Team Team;

		public DamageInfo(float damage, Damage source, Team team)
		{
			Damage = damage;
			Source = source;
			Team = team;
		}
	}
}
