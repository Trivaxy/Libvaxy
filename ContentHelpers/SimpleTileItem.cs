using Terraria.ID;

namespace Libvaxy.ContentHelpers
{
	public class SimpleTileItem : SimpleItem
	{
		private readonly int placeType;

		public SimpleTileItem(string name, string toolTip, int maxStack, int value, int rarity, int placeType) : base(name, toolTip, maxStack, value, rarity)
			=> this.placeType = placeType;

		public SimpleTileItem(string name, int maxStack, int value, int rarity, int placeType) : base(name, maxStack, value, rarity)
			=> this.placeType = placeType;

		public SimpleTileItem(string name, int maxStack, int value, int placeType) : base(name, maxStack, value)
			=> this.placeType = placeType;

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.createTile = placeType;
			item.useTurn = true;
			item.autoReuse = true;
			item.consumable = true;
			item.useTime = 15;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
		}
	}
}