using System;
using System.Collections.Generic;
using System.Linq;
using Loonge.Api.IO;

namespace Loonge.Api.Lexing
{
	public class Lexer : ILexer
	{
		public int Line => _input.Line;
		public int Column => _input.Column;
		public int Position => _input.Position;

		private InputStream _input;

		private Token _lastToken;
		private char _lastChar = ' ';

		private static bool IsDelimiter(char ch) => ",.".Contains(ch);

		private static List<string> _keywords = new List<string>();
		private static List<string> _typeAliases = new List<string>();
		private static bool IsOperator(char ch) => "+-/*=^!~?:<>&|%".Contains(ch);

		private static Dictionary<string, Operator> _operators = new()
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
			{ "?", Operator.TernaryFirst },
			{ ":", Operator.TernarySecond },
			{ ">", Operator.Greater },
			{ "<", Operator.Less },
			//{ "+", Operator.StringInterpolation },
			{ "!", Operator.LogicalNot },
			{ "=>", Operator.Lambda },
			{ "==", Operator.Equals },
			{ "!=", Operator.NotEquals },
			{ ">=", Operator.GreaterOrEquals },
			{ "<=", Operator.LessOrEquals },
			{ "+=", Operator.PlusAssign },
			{ "-=", Operator.MinusAssign },
			{ "*=", Operator.MultiplyAssign },
			{ "/=", Operator.DivideAssign },
			{ "%=", Operator.ModuleAssign },
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

		private void ReadNoEof()
		{
			if (!_input.IsEndOfStream)
				_lastChar = _input.Read();
		}

		public Lexer(InputStream input)
		{
			_input = input;
		}

		static Lexer()
		{
			_keywords = new List<string>();
			_keywords.AddRange(Enum.GetNames<Keyword>().Select(s => s.ToLowerInvariant()));

			_typeAliases = new List<string>();
			_typeAliases.AddRange(Enum.GetNames<TypeAlias>().Select(s => s.ToLowerInvariant()));

			_stringOperatorMap = new Dictionary<string, Operator>();
			foreach (var op in Enum.GetValues<Operator>())
			{
				_stringOperatorMap[op.ToString().ToLowerInvariant()] = op;
			}
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

			if (_lastChar == '\"') // String
			{
				return ReadString();
			}

			if (_lastChar == '\'') // Char
			{
				return ReadChar();
			}

			if (_lastChar == '.' || char.IsDigit(_lastChar)) // Number
			{
				return ReadNumber();
			}

			if (IsOperator(_lastChar))
			{
				return ReadOperator();
			}

			if (char.IsLetter(_lastChar))
			{
				// Type Alias, Keyword or Identifier
				
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
			throw exception;
		}

		private Token ReadNumber()
		{
			if (_lastChar == '.') // That's a decimal number with omitted 0
			{
				var numStr = ".";
				
				do
				{
					_lastChar = _input.Read();
					numStr += _lastChar;
				} while (char.IsDigit(_lastChar));

				return new Token(TokenType.DecimalNumber, double.Parse(numStr));
			}

			bool pointFound = false;
			var numStr2 = "" + _lastChar;
			
			do
			{
				_lastChar = _input.Read();
				numStr2 += _lastChar;
				if (_lastChar == '.')
				{
					if (!pointFound)
						pointFound = true;
					else
						throw new SyntaxException("Multiple points in number", Position, Line, Column);
				}
					
			} while (char.IsDigit(_lastChar) || _lastChar == '.');

			// TODO: Add support for postfixes (f for float, m for decimal, etc)
			if (pointFound)
				return new Token(TokenType.DecimalNumber, double.Parse(numStr2));

			return new Token(TokenType.Number, int.Parse(numStr2));
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
					nextChar = _input.Peek();
					if (nextChar == 'n')
						str += '\n';
					else if (nextChar == 't')
						str += '\t';
					else if (nextChar == 'r')
						str += '\r';
					else if (nextChar == '\'')
						str += '\'';
					else if (nextChar == '\"')
						str += '\"';
					else if (nextChar == '\\')
						str += '\\';
					else if (nextChar == '0')
						str += '\0';
					else if (nextChar == 'a')
						str += '\a';
					else if (nextChar == 'b')
						str += '\b';
					else if (nextChar == 'v')
						str += '\v';
					else if (nextChar == 'f')
						str += '\f';
					else
						throw new SyntaxException($"Invalid special char after backslash: {nextChar}", 
							Position, Line, Column);

					_lastChar = _input.Read();
					continue;
				}
				
				if (_lastChar != '\"')
					str += _lastChar;
				else
					break;
			}
			while (!_input.IsEndOfStream && _lastChar != '\n' && _lastChar != '\r'/* && _lastChar != '\"'*/);

			if (_lastChar != '\"')
				throw new SyntaxException("String never closes on the same line", Position, Line, Column);

			return new Token(TokenType.String, str);
		}

		private Token ReadChar()
		{
			// Character can always contain only 3 symbols: 
			// - 2 single quote marks
			// - 1 char
			_lastChar = _input.Read(); // this is our char
			var quote = _input.Read(); // this is the closing quote
			if (quote != '\'')
				throw new SyntaxException("Only one character expected", Position, Line, Column);

			return new Token(TokenType.Character, _lastChar);
		}

		private Token ReadOperator()
		{
			var next = _input.Peek();
			//if (next == '=')
			//	return new Token(TokenType.Operator, _lastChar + '=');

			// TODO: Only single and double symbol operators currently supported
			var multi = "" + _lastChar + next;
			if (_operators.ContainsKey(multi))
			{
				_lastChar = _input.Read();
				return new Token(TokenType.Operator, _operators[multi]);
			}
			
			return new Token(TokenType.Operator, _operators[_lastChar.ToString()]);
		}

		private static Dictionary<string, Operator> _stringOperatorMap = new Dictionary<string, Operator>();
		
		private (bool isValid, Operator? op) IsValidMultiOp()
		{
			var nextChar = _input.Peek();

			//if (nextChar == '=') // += -= *= etc
			//	return "+-/*&|%^=!".Contains(_lastChar);

			if (nextChar == '&')
				return (_lastChar == '&', Operator.LogicalAnd); // i && j

			if (nextChar == '|')
				return (_lastChar == '|', Operator.LogicalOr); // i || j

			if (nextChar == '+')
				return (_lastChar == '+', Operator.Increment); // i++

			if (nextChar == '-')
				return (_lastChar == '-', Operator.Decrement); // i--

			return (false, null);
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
