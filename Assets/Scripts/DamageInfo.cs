namespace Quinn
{
	[System.Serializable]
	public class DamageInfo
	{
		public float Damage;
		public Damage Source;
		public ElementType Element;

		public DamageInfo(float damage, Damage source, ElementType element)
		{
			Damage = damage;
			Source = source;
			Element = element;
		}
	}
}
