namespace Quinn
{
	[System.Serializable]
	public class DamageInfo
	{
		public float Damage;
		public Damage Source;

		public DamageInfo(float damage, Damage source)
		{
			Damage = damage;
			Source = source;
		}
	}
}
