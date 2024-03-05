using UnityEngine;

namespace Quinn.AI
{
	[System.Serializable]
	public class Meter
	{
		public const float Max = 100f;

		[SerializeField]
		private AnimationCurve SampleCurve;

		public float Factor => SampleCurve.Evaluate(_value / Max);

		private float _value;

		public static Meter operator+(Meter a, float b)
		{
			a.Add(b);
			return a;
		}

		public static Meter operator -(Meter a, float b)
		{
			a.Remove(b);
			return a;
		}

		public void Add(float amount)
		{
			_value = Mathf.Min(_value + amount, Max);
		}

		public void Remove(float amount)
		{
			_value = Mathf.Max(0f, _value - amount);
		}

		public override string ToString() => $"{_value}%";
	}
}
