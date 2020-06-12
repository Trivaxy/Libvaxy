using System.Security.Principal;
using Terraria.ID;
using Terraria.ModLoader;

namespace Libvaxy.GameHelpers
{
	public class ModTileItem : ModItem
	{
		private readonly string name;
		private readonly string tooltip;
		private readonly int placeType;
		private readonly int rarity;
		private readonly int maxStack;
		private readonly int value;

		public ModTileItem(string name, int placeType)
		{
			this.name = name;
			this.placeType = placeType;
			this.rarity = ItemRarityID.White;
			this.maxStack = 999;
			this.value = 5;
		}

		public ModTileItem(string name, int placeType, int rarity)
		{
			this.name = name;
			this.placeType = placeType;
			this.rarity = rarity;
			this.maxStack = 999;
			this.value = 5;
		}

		public ModTileItem(string name, string tooltip, int placeType)
		{
			this.name = name;
			this.tooltip = tooltip;
			this.placeType = placeType;
			this.rarity = ItemRarityID.White;
			this.maxStack = 999;
			this.value = 5;
		}

		public ModTileItem(string name, string tooltip, int placeType, int rarity, int maxStack, int value)
		{
			this.name = name;
			this.tooltip = tooltip;
			this.placeType = placeType;
			this.rarity = rarity;
			this.maxStack = maxStack;
			this.value = value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(name);

			if (!string.IsNullOrEmpty(tooltip))
				Tooltip.SetDefault(tooltip);
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.createTile = placeType;
			item.rare = rarity;
			item.maxStack = maxStack;
			item.value = value;
			item.useTurn = true;
			item.autoReuse = true;
			item.consumable = true;
			item.useTime = 15;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
		}
	}
}
