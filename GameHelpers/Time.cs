using Terraria;

namespace Libvaxy.GameHelpers
{
	public static class Time
	{
		public const int DayLength = 54000;
		public const int NightLength = 32400;

		public static int FromMilliSeconds(float milliseconds) => FromSeconds(milliseconds / 1000);

		public static int FromSeconds(float seconds) => (int)(seconds * 60);

		public static int FromMinutes(float minutes) => FromSeconds(minutes) * 60;

		public static int FromHours(float hours) => FromMinutes(hours) * 60;

		public static void SetToDusk()
		{
			Main.dayTime = false;
			Main.time = 0;
		}

		public static void SetToDawn()
		{
			Main.dayTime = true;
			Main.time = 0;
		}

		public static void SetToNoon()
		{
			Main.dayTime = true;
			Main.time = DayLength / 2;
		}

		public static void SetToMidnight()
		{
			Main.dayTime = false;
			Main.time = NightLength / 2;
		}
	}
}
