﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Libvaxy
{
	public static class TextureExtensions
	{
		// TODO: make this not call GetColors() every single time
		/// <summary>
		/// Sets the color of the texture's pixel in a specified coordinate.
		/// </summary>
		/// <param name="x">The X coordinate of the pixel.</param>
		/// <param name="y">The Y coordinate of the pixel.</param>
		/// <param name="color">The color to set the pixel to.</param>
		public static void SetColor(this Texture2D texture, int x, int y, Color color)
		{
			Color[] colors = texture.GetColors();
			colors[CoordinateToIndex(x, y, texture.Width)] = color;
			texture.SetData(colors);
		}

		/// <summary>
		/// Sets a rectangle of pixels in a texture to a certain color.
		/// </summary>
		/// <param name="rect">The rectangle within the texture.</param>
		/// <param name="color">The color to set the rectangle to.</param>
		public static void SetColorsRect(this Texture2D texture, Rectangle rect, Color color)
		{
			Color[] colors = texture.GetColors();

			for (int x = rect.X; x < rect.X + rect.Width; x++)
				for (int y = rect.Y; y < rect.Y + rect.Height; y++)
					colors[CoordinateToIndex(x, y, texture.Width)] = color;

			texture.SetData(colors);
		}

		/// <summary>
		/// Fills an entire texture with a color.
		/// </summary>
		/// <param name="color">The color to set the texture to.</param>
		public static void Fill(this Texture2D texture, Color color)
		{
			Color[] colors = texture.GetColors();

			for (int i = 0; i < colors.Length; i++)
				colors[i] = color;

			texture.SetData(colors);
		}

		/// <summary>
		/// Returns all the colors in a texture.
		/// </summary>
		public static Color[] GetColors(this Texture2D texture)
		{
			Color[] colors = new Color[texture.Width * texture.Height];
			texture.GetData(colors);
			return colors;
		}

		/// <summary>
		/// Returns all the colors within a specified rectangle in a texture.
		/// </summary>
		/// <param name="rect">The rectangle within the texture.</param>
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

		/// <summary>
		/// Gets the average color in the texture, ignoring transparency.
		/// </summary>
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

		/// <summary>
		/// Premultiplies a texture.
		/// </summary>
		public static void PreMultiply(this Texture2D texture)
		{
			Color[] colors = texture.GetColors();

			for (int i = 0; i < colors.Length; i++)
				colors[i] = Color.FromNonPremultiplied(colors[i].R, colors[i].G, colors[i].B, colors[i].A);

			texture.SetData(colors);
		}

		/// <summary>
		/// Clones a certain texture and returns a new Texture2D object for it. Libvaxy will automatically dispose the cloned texture on unload.
		/// </summary>
		/// <returns>The cloned texture.</returns>
		public static Texture2D Clone(this Texture2D texture)
		{
			Color[] colors = texture.GetColors();
			Texture2D newTexture = new Texture2D(Main.instance.GraphicsDevice, texture.Width, texture.Height);
			newTexture.SetData(colors);
			LibvaxyMod.DisposeOnUnload(newTexture);
			return newTexture;
		}

		/// <summary>
		/// Clones a certain rectangle within a texture and returns a new Texture2D object for it. Libvaxy will automatically dispose the cloned texture on unload.
		/// </summary>
		/// <param name="rect">The rectangle within the texture to clone.</param>
		/// <returns>The cloned texture rectangle.</returns>
		public static Texture2D CloneRectangle(this Texture2D texture, Rectangle rect)
		{
			Texture2D newTexture = new Texture2D(Main.instance.GraphicsDevice, rect.Width, rect.Height);
			Color[] colors = texture.GetColorsRect(rect);
			newTexture.SetData(colors);
			LibvaxyMod.DisposeOnUnload(newTexture);
			return newTexture;
		}

		/// <summary>
		/// Returns a rectangle specifying the texture's dimensions.
		/// </summary>
		public static Rectangle GetDimensions(this Texture2D texture) => new Rectangle(0, 0, texture.Width, texture.Height);

		/// <summary>
		/// Masks a texture with another texture's transparency. Transparent pixels in the alpha mask will have their alpha value placed onto the masked texture.
		/// </summary>
		/// <param name="alphaMask">The alpha mask texture.</param>
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

		/// <summary>
		/// Merges a texture with another texture by interpolating between them both. You can optionally specify whether transparent pixels are interpolated or ignored (ignored by default).
		/// </summary>
		/// <param name="otherTexture">The texture to merge with.</param>
		/// <param name="allowAlpha">Whether transparent pixels are interpolated as well or ignored. This is false by default.</param>
		public static void MergeTexture(this Texture2D texture, Texture2D otherTexture, bool allowAlpha = false)
		{
			CheckMasksCompatible(texture, otherTexture);

			Color[] textureColors = texture.GetColors();
			Color[] otherTextureColors = otherTexture.GetColors();

			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					int index = CoordinateToIndex(x, y, texture.Width);
					Color otherTextureColor = otherTextureColors[index];

					if (otherTextureColor.A != 255 && !allowAlpha)
						continue;

					textureColors[index] = Color.Lerp(textureColors[index], otherTextureColor, 0.5f);
				}
			}

			texture.SetData(textureColors);

			if (allowAlpha)
				texture.PreMultiply();
		}

		private static int CoordinateToIndex(int x, int y, int width) => y * width + x;

		private static void CheckMasksCompatible(Texture2D texture, Texture2D mask)
		{
			if (texture.Width != mask.Width || texture.Height != mask.Height)
				throw new LibvaxyException("Attempted to mask alpha texture with an incompatible size");
		}
	}
}