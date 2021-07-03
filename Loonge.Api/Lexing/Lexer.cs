using System;
using Loonge.Api.IO;
using Loonge.Api.Lexing;

namespace Loonge.Lexing
{
	public class Lexer : ILexer
	{
		public int Line => _input.Line;
		public int Column => _input.Column;
		public int Position => _input.Position;

		private InputStream _input;

		private Token _lastToken;
		private char _lastChar = ' ';

		private void ReadNoEof()
		{
			if (!_input.IsEndOfStream)
				_lastChar = _input.Read();
		}

		public Lexer(InputStream input)
		{
			_input = input;
		}

		public Token Read()
		{
			if (_input.IsEndOfStream)
				return new Token(TokenType.Eof, null);
			
			// Skip all whitespace characters and semicolons
			do _lastChar = _input.Read();
			while (char.IsWhiteSpace(_lastChar) && !_input.IsEndOfStream);
			
			if (_lastChar == '/') // can be either an operator or comment
			{
				var nextChar = _input.Peek();
				if (nextChar == '/') // Single line Comment (//)
				{
					SkipComment();
				}
				else if (nextChar == '*') // Multi line comment (/*)
				{
					SkipMultiLineComment();
				}
				else // An operator 
				{
					return ReadOperator();
				}
				
				return Read();
			}

			throw new SyntaxException(
				$"Unexpected character: {_lastChar} (code: {(int) _lastChar})",
				_input.Position,
				_input.Line,
				_input.Column
			);
		}

		public Token Peek()
		{
			throw new NotImplementedException();
		}

		public void ThrowException(Exception exception)
		{
			throw new NotImplementedException();
		}

		private Token ReadString()
		{
			throw new NotImplementedException();
		}

		private Token ReadOperator()
		{
			var next = _input.Peek();
			if (next == '=')
				return new Token(TokenType.Operator, _lastChar + '=');

			return new Token(TokenType.Operator, _lastChar.ToString());
		}

		private void SkipComment()
		{
			do _lastChar = _input.Read();
			while (!_input.IsEndOfStream && _lastChar != '\n' && _lastChar != '\r');
		}

		private void SkipMultiLineComment()
		{
			while (true)
			{
				var ch = _input.Read();
				if (_input.IsEndOfStream || (ch == '*' && _input.Peek() == '/'))
				{
					if (!_input.IsEndOfStream)
						_lastChar = _input.Read();
					return;
				}
			}
			
		}
	}
}
