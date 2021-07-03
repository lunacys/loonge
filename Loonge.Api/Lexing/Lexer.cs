using System;
using Loonge.Api.IO;

namespace Loonge.Lexing
{
	public class Lexer : ILexer
	{
		public int Line => _input.Line;
		public int Column => _input.Column;

		private InputStream _input;

		public Lexer(InputStream input)
		{
			_input = input;
		}

		public Token Read()
		{
			throw new NotImplementedException();
		}

		public Token Peek()
		{
			throw new NotImplementedException();
		}

		public void ThrowException(Exception exception)
		{
			throw new NotImplementedException();
		}
	}
}
