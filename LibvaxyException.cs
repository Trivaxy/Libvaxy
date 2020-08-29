using System;

namespace Libvaxy
{
	/// <summary>
	/// Represents the exception Libvaxy throws internally.
	/// </summary>
	[Serializable]
	internal class LibvaxyException : Exception
	{
		public LibvaxyException(string message) : base(message)
		{
		}
	}
}