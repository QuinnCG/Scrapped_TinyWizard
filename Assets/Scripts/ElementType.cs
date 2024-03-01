using System;

namespace Quinn
{
	[Flags]
	public enum ElementType
	{
		Fire = 1,
		Water = 2,
		Earth = 4,
		Lightning = 8,
		Holy = 16,
		Nature = 32,
		Gravity = 64
	}
}
