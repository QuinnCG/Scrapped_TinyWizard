using Sirenix.OdinInspector;

namespace Quinn.AI
{
	[System.Serializable]
	public class AIRand
	{
		[HorizontalGroup]
		public float Mean;

		[HorizontalGroup]
		public float Deviation;

		public AIRand(float mean, float deviation = 0f)
		{
			Mean = mean;
			Deviation = deviation;
		}

		public static implicit operator AIRand(float value) => new(value);
		public static implicit operator AIRand((float, float) value) => new(value.Item1, value.Item2);
	}
}
