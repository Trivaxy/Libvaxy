using System;

namespace Libvaxy
{
	[Serializable]
	internal class LibvaxyException : Exception
	{
		public LibvaxyException(string message) : base(message)
		{
		}
	}
}