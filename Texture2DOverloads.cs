using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Libvaxy
{
	public static class Texture2DOverloads
	{
		// TODO: make this not call GetColors() every single time
		public static void SetColor(this Texture2D texture, int x, int y, Color color)
		{
			Color[] colors = texture.GetColors();
			colors[CoordinateToIndex(x, y, texture.Width)] = color;
			texture.SetData(colors);
		}

		public static void SetColorsRect(this Texture2D texture, Rectangle rect, Color color)
		{
			Color[] colors = texture.GetColors();

			for (int x = rect.X; x < rect.X + rect.Width; x++)
				for (int y = rect.Y; y < rect.Y + rect.Height; y++)
					colors[CoordinateToIndex(x, y, texture.Width)] = color;

			texture.SetData(colors);
		}

		public static void Fill(this Texture2D texture, Color color)
		{
			Color[] colors = texture.GetColors();

			for (int i = 0; i < colors.Length; i++)
				colors[i] = color;

			texture.SetData(colors);
		}

		public static Color[] GetColors(this Texture2D texture)
		{
			Color[] colors = new Color[texture.Width * texture.Height];
			texture.GetData(colors);
			return colors;
		}

		public static Color[] GetColorsRect(this Texture2D texture, Rectangle rect)
		{
			if (!texture.GetDimensions().Contains(rect))
				throw new LibvaxyException("Attempted to get texture colors with a non-fully contained rectangle");

			Color[] colors = texture.GetColors();
			List<Color> rectColors = new List<Color>();

			for (int x = rect.X; x < rect.X + rect.Width; x++)
				for (int y = rect.Y; y < rect.Y + rect.Height; y++)
					rectColors.Add(colors[CoordinateToIndex(x, y, texture.Width)]);

			return rectColors.ToArray();
		}

		public static Color GetAverageColor(this Texture2D texture)
		{
			Color[] colors = texture.GetColors();
			int r = 0;
			int g = 0;
			int b = 0;
			int countedColors = colors.Length;

			for (int i = 0; i < colors.Length; i++)
			{
				if (colors[i].A != 255)
				{
					countedColors--;
					continue;
				}

				r += colors[i].R;
				g += colors[i].G;
				b += colors[i].B;
			}

			return new Color((byte)(r / countedColors), (byte)(g / countedColors), (byte)(b / countedColors));
		}

		public static void PreMultiply(this Texture2D texture)
		{
			Color[] colors = texture.GetColors();

			for (int i = 0; i < colors.Length; i++)
				colors[i] = Color.FromNonPremultiplied(colors[i].R, colors[i].G, colors[i].B, colors[i].A);

			texture.SetData(colors);
		}

		public static Texture2D Clone(this Texture2D texture)
		{
			Color[] colors = texture.GetColors();
			Texture2D newTexture = new Texture2D(Main.instance.GraphicsDevice, texture.Width, texture.Height);
			newTexture.SetData(colors);
			LibvaxyMod.DisposeOnUnload(newTexture);
			return newTexture;
		}

		public static Texture2D CloneRectangle(this Texture2D texture, Rectangle rect)
		{
			Texture2D newTexture = new Texture2D(Main.instance.GraphicsDevice, rect.Width, rect.Height);
			Color[] colors = texture.GetColorsRect(rect);
			newTexture.SetData(colors);
			LibvaxyMod.DisposeOnUnload(newTexture);
			return newTexture;
		}

		public static Rectangle GetDimensions(this Texture2D texture) => new Rectangle(0, 0, texture.Width, texture.Height);

		public static void MaskAlpha(this Texture2D texture, Texture2D alphaMask)
		{
			CheckMasksCompatible(texture, alphaMask);

			Color[] textureColors = texture.GetColors();
			Color[] alphaMaskColors = alphaMask.GetColors();

			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					int index = CoordinateToIndex(x, y, texture.Width);
					byte alpha = alphaMaskColors[index].A;

					if (alpha != 255)
						textureColors[index].A = alpha;
				}
			}

			texture.SetData(textureColors);
			texture.PreMultiply();
		}

		public static void MaskTexture(this Texture2D texture, Texture2D mask, bool allowAlpha = false)
		{
			CheckMasksCompatible(texture, mask);

			Color[] textureColors = texture.GetColors();
			Color[] maskColors = mask.GetColors();

			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					int index = CoordinateToIndex(x, y, texture.Width);
					Color maskColor = maskColors[index];

					if (maskColor.A != 255 && !allowAlpha)
						continue;

					textureColors[index] = Color.Lerp(textureColors[index], maskColor, 0.5f);
				}
			}

			texture.SetData(textureColors);

			if (allowAlpha)
				texture.PreMultiply();
		}

		public static int CoordinateToIndex(int x, int y, int width) => y * width + x;

		private static void CheckMasksCompatible(Texture2D texture, Texture2D mask)
		{
			if (texture.Width != mask.Width || texture.Height != mask.Height)
				throw new LibvaxyException("Attempted to mask alpha texture with an incompatible size");
		}
	}
}