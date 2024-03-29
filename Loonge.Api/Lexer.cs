﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Loonge.Api.Exceptions;
using Loonge.Api.IO;
using Loonge.Api.TokenData;

namespace Loonge.Api
{
	public class Lexer : ILexer
	{
		public int Line => _input.Line;
		public int Column => _input.Column;
		public int Position => _input.Position;

		private readonly InputStream _input;

		private Token _lastToken;
		private char _lastChar = ' ';

		public static bool IsPunctuation(char ch) => ",;[](){}".Contains(ch);

		private static readonly List<string> Keywords;
		private static readonly List<string> TypeAliases;
		public static bool IsOperator(char ch) => "+-/*=^!~?:<>&|%.$@".Contains(ch);
		public static bool IsValidWordPart(char ch) => char.IsLetterOrDigit(ch) || ch == '_';

		public static readonly Dictionary<string, Operator> Operators = new()
		{
			{ "+", Operator.Plus },
			{ "-", Operator.Minus },
			{ "/", Operator.Divide },
			{ "*", Operator.Multiply },
			{ "%", Operator.Modulo },
			{ "^", Operator.BitXor },
			{ "|", Operator.BitOr },
			{ "&", Operator.BitAnd },
			{ "~", Operator.BitNot },
			{ "=", Operator.Assign },
			{ ":", Operator.TypeAssign },
			{ "?", Operator.Nullable},
			{ ">", Operator.Greater },
			{ "<", Operator.Less },
			{ ".", Operator.MemberAccess },
			{ "$", Operator.StringInterpolation },
			{ "@", Operator.StringAsIs},
			{ "!", Operator.LogicalNot },
			{ "::", Operator.ModuleProvider},
			{ "=>", Operator.Lambda },
			{ "->", Operator.Next },
			{ "==", Operator.Equals },
			{ "!=", Operator.NotEquals },
			{ ">=", Operator.GreaterOrEquals },
			{ "<=", Operator.LessOrEquals },
			{ "+=", Operator.PlusAssign },
			{ "-=", Operator.MinusAssign },
			{ "*=", Operator.MultiplyAssign },
			{ "/=", Operator.DivideAssign },
			{ "%=", Operator.ModuloAssign },
			{ "^=", Operator.BitXorAssign },
			{ "|=", Operator.BitOrAssign },
			{ "&=", Operator.BitAndAssign },
			{ ">>", Operator.BitShiftRight },
			{ "<<", Operator.BitShiftLeft },
			{ "&&", Operator.LogicalAnd },
			{ "||", Operator.LogicalOr },
			{ "++", Operator.Increment },
			{ "--", Operator.Decrement },
		};

        public Lexer(InputStream input)
		{
			_input = input;
		}

		static Lexer()
		{
			Keywords = new List<string>();
			Keywords.AddRange(Enum.GetNames<Keyword>().Select(s => s.ToLowerInvariant()));

			TypeAliases = new List<string>();
			TypeAliases.AddRange(Enum.GetNames<TypeAlias>().Select(s => s.ToLowerInvariant()));
		}

		public Token Read()
		{
			if (_input.IsEndOfStream)
				return _lastToken = new Token(TokenType.Eof, null);
			
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
					return _lastToken = ReadOperator();
				}
				
				return _lastToken = Read();
			}

			if (_lastChar == '\"') // String
                return _lastToken = ReadString();

            if (_lastChar == '\'') // Char
                return _lastToken = ReadChar();

            if (char.IsDigit(_lastChar) || (_lastChar == '.' && char.IsDigit(_input.Peek()))) // Number
                return _lastToken = ReadNumber();
			
			if (IsOperator(_lastChar))
                return _lastToken = ReadOperator();

            // Type Alias, Keyword or Identifier
			if (char.IsLetter(_lastChar) || _lastChar == '_')
                return _lastToken = ReadWord();

            if (IsPunctuation(_lastChar))
                return _lastToken = ReadPunctuation();

            throw GetException($"Unexpected token: {_lastChar} (char code: {(int)_lastChar})");
		}

		public Token Peek()
		{
			return _lastToken == null ? Read() : _lastToken;
		}

		public Exception GetException(string additionalMessage = null, Exception exception = null)
        {
            if (exception == null)
                return new SyntaxException(additionalMessage, Position, Line, Column);
			
			return new SyntaxException(exception.Message + $"\nAdditional: {additionalMessage}", Position, Line, Column);
		}

        public void ThrowException(string additionalMessage = null, Exception innerException = null)
        {
            throw GetException(additionalMessage, innerException);
        }

		private Token ReadPunctuation()
		{
			return new Token(TokenType.Punctuation, _lastChar);
		}

		private Token ReadWord()
		{
			var word = "";

			while (true)
			{
				word += _lastChar;
				if (!IsValidWordPart(_input.Peek()))
					break;

				_lastChar = _input.Read();
			}

			if (TypeAliases.Contains(word))
				return new Token(TokenType.TypeAlias, Enum.Parse<TypeAlias>(word, true));

			if (Keywords.Contains(word))
				return new Token(TokenType.Keyword, Enum.Parse<Keyword>(word, true));

			return new Token(TokenType.Identifier, word);
		}

		private Token ReadNumber()
		{
			if (_lastChar == '.') // That's a decimal number with omitted 0
			{
				var numStr = "0";
				
				while (true)
				{
					numStr += _lastChar;

					if (!char.IsDigit(_input.Peek()))
						break;
				
					_lastChar = _input.Read();
				}

				double parsedD;

				try
				{
					parsedD = double.Parse(numStr, CultureInfo.InvariantCulture);
				}
				catch (FormatException)
				{
					throw GetException($"Invalid number: '{numStr}'");
				}

				return new Token(TokenType.DecimalNumber, parsedD);
			}

			bool pointFound = false;
			var numStr2 = "";

			while (true)
			{
				numStr2 += _lastChar;

				if (!char.IsDigit(_input.Peek()) && _input.Peek() != '.')
					break;
				
				_lastChar = _input.Read();
				if (_lastChar == '.')
				{
					if (!pointFound)
						pointFound = true;
					else
						throw GetException("Multiple points in number");
				}
			}

			// TODO: Add support for postfixes (f for float, m for decimal, etc)
			if (pointFound)
			{
				double parsedD2;

				try
				{
					parsedD2 = double.Parse(numStr2, CultureInfo.InvariantCulture);
				}
				catch (FormatException)
				{
					throw GetException($"Invalid number: '{numStr2}'");
				}
				
				return new Token(TokenType.DecimalNumber, parsedD2);
			}

			int parsed;

			try
			{
				parsed = int.Parse(numStr2, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw GetException($"Invalid number: '{numStr2}'");
			}

			return new Token(TokenType.Number, parsed);
		}

		private Token ReadString()
		{
			var nextChar = _input.Peek();
			if (nextChar == '\"') // Empty string
			{
				_lastChar = _input.Read();
				return new Token(TokenType.String, "");
			}

			var str = "";
			
			do
			{
				_lastChar = _input.Read();
				if (_lastChar == '\\')
				{
					if (!TryReadControlChar(out var ch))
                        throw GetException($"Invalid special char after backslash: {ch.Value}");

                    str += ch.Value;

					_lastChar = _input.Read();
					continue;
				}
				
				if (_lastChar != '\"')
					str += _lastChar;
				else
					break;
			}
			while (!_input.IsEndOfStream && _lastChar != '\n' && _lastChar != '\r');

			if (_lastChar != '\"')
				throw GetException("String never closes on the same line");

			return new Token(TokenType.String, str);
		}

		private Token ReadChar()
		{
			// Character can always contain only 3 OR 4 symbols (4 if it is a control char): 
			// - 2 single quote marks
			// - 1 char
			_lastChar = _input.Read(); // this is our char

			if (_lastChar == '\\') // control char
			{
				if (TryReadControlChar(out var ch))
				{
					_lastChar = _input.Read(); // this is second char
					_lastChar = _input.Read(); // this is the closing quote
					return new Token(TokenType.Character, ch.Value);	
				}

				throw GetException($"Invalid special char after backslash: {_input.Peek()}");
			}
			
			var quote = _input.Read(); // this is the closing quote
			if (quote != '\'')
				throw GetException("Only one character expected");

			return new Token(TokenType.Character, _lastChar);
		}

		private bool TryReadControlChar(out char? ch)
        {
            var nextChar = _input.Peek();
			if (nextChar == 'n')
				ch = '\n';
			else if (nextChar == 't')
				ch = '\t';
			else if (nextChar == 'r')
				ch = '\r';
			else if (nextChar == '\'')
				ch = '\'';
			else if (nextChar == '\"')
				ch = '\"';
			else if (nextChar == '\\')
				ch = '\\';
			else if (nextChar == '0')
				ch = '\0';
			else if (nextChar == 'a')
				ch = '\a';
			else if (nextChar == 'b')
				ch = '\b';
			else if (nextChar == 'v')
				ch = '\v';
			else if (nextChar == 'f')
				ch = '\f';
			else
            {
                ch = nextChar;
                return false;
            }

			return true;
		}

		private Token ReadOperator()
		{
			var next = _input.Peek();

			// TODO: Only single and double symbol operators currently supported
			var multi = "" + _lastChar + next;
			if (Operators.ContainsKey(multi))
			{
				_lastChar = _input.Read();
				return new Token(TokenType.Operator, Operators[multi]);
			}
			
			return new Token(TokenType.Operator, Operators[_lastChar.ToString()]);
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
