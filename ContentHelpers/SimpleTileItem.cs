using Terraria.ID;

namespace Libvaxy.ContentHelpers
{
	/// <summary>
	/// A simple tile-placing item that needs nothing but a name, maximum stack, value and placed tile type. Extra attributes can be specified through different constructors.
	/// Using this class is as simple as extending from it and calling the base constructor with the attributes you want.
	/// </summary>
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