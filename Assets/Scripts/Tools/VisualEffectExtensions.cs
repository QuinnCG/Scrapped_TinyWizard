using UnityEngine.VFX;

namespace Quinn
{
	public static class VisualEffectExtensions
	{
		public static void DestroyOnFinish(this VisualEffect vfx)
		{
			DestroyOnCondition.Create(vfx.transform, () => vfx.aliveParticleCount == 0);
		}
	}
}
