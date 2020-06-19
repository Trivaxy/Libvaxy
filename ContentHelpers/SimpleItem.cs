using Terraria.ID;
using Terraria.ModLoader;

namespace Libvaxy.ContentHelpers
{
	public class SimpleItem : ModItem
	{
		private readonly string name;
		private readonly string toolTip;
		private readonly int maxStack;
		private readonly int value;
		private readonly int rarity;

		public SimpleItem(string name, string toolTip, int maxStack, int value, int rarity)
		{
			this.name = name;
			this.toolTip = toolTip;
			this.maxStack = maxStack;
			this.value = value;
			this.rarity = rarity;
		}

		public SimpleItem(string name, int maxStack, int value, int rarity)
		{
			this.name = name;
			this.toolTip = null;
			this.maxStack = maxStack;
			this.value = value;
			this.rarity = rarity;
		}

		public SimpleItem(string name, int maxStack, int value)
		{
			this.name = name;
			this.toolTip = null;
			this.maxStack = maxStack;
			this.value = value;
			this.rarity = ItemRarityID.White;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(name);

			if (toolTip != null)
				Tooltip.SetDefault(toolTip);
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.rare = rarity;
			item.maxStack = maxStack;
			item.value = value;
		}
	}
}
