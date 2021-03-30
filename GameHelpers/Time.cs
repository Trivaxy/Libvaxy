using Terraria;

namespace Libvaxy.GameHelpers
{
	/// <summary>
	/// Contains useful functions related to in-game time.
	/// </summary>
	public static class Time
	{
		public const int DayLength = 54000;
		public const int NightLength = 32400;

		/// <summary>
		/// Converts milliseconds to ticks.
		/// </summary>
		/// <param name="milliseconds">How many milliseconds.</param>
		/// <returns>The number of ticks from milliseconds.</returns>
		public static int FromMilliseconds(float milliseconds) => FromSeconds(milliseconds / 1000);

		/// <summary>
		/// Converts seconds to ticks.
		/// </summary>
		/// <param name="seconds">How many seconds.</param>
		/// <returns>The number of ticks from seconds.</returns>
		public static int FromSeconds(float seconds) => (int)(seconds * 60);

		/// <summary>
		/// Converts minutes to ticks.
		/// </summary>
		/// <param name="minutes">How many minutes.</param>
		/// <returns>The number of ticks from minutes.</returns>
		public static int FromMinutes(float minutes) => FromSeconds(minutes) * 60;

		/// <summary>
		/// Converts hours to ticks.
		/// </summary>
		/// <param name="hours">How many hours.</param>
		/// <returns>The number of ticks from hours.</returns>
		public static int FromHours(float hours) => FromMinutes(hours) * 60;

		/// <summary>
		/// Sets in-game time to dusk.
		/// </summary>
		public static void SetToDusk()
		{
			Main.dayTime = false;
			Main.time = 0;
		}

		/// <summary>
		/// Sets in-game time to dawn.
		/// </summary>
		public static void SetToDawn()
		{
			Main.dayTime = true;
			Main.time = 0;
		}

		/// <summary>
		/// Sets in-game time to noon.
		/// </summary>
		public static void SetToNoon()
		{
			Main.dayTime = true;
			Main.time = DayLength / 2;
		}

		/// <summary>
		/// Sets in-game time to midnight.
		/// </summary>
		public static void SetToMidnight()
		{
			Main.dayTime = false;
			Main.time = NightLength / 2;
		}
	}
}
