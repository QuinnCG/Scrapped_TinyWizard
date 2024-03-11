namespace Quinn.AI.Enemies
{
	public class Ghoul : Enemy
	{
		protected override Tree ConstructTree()
		{
			return new Tree()
			{
				new Tasks.MoveTo(Player.transform, 3f)
			};
		}
	}
}
