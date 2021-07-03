using System;

namespace Loonge.Lexing
{
	[Serializable]
	public class InvalidTokenException : Exception
	{
		public int Line { get; }
		public int Column { get; }

		public InvalidTokenException(int line, int column)
			: this(string.Empty, line, column)
		{ }

		public InvalidTokenException(string message, int line, int column)
			: base(message)
		{
			Line = line;
			Column = column;
		}

		public override string ToString()
		{
			return $"{Message} ({Line}:{Column})";
		}
	}
}
