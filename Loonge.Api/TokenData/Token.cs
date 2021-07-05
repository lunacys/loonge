using System;

namespace Loonge.Api.TokenData
{
	public class Token : IEquatable<Token>
	{
		public TokenType Type { get; }
		public object Value { get; }

		public Token(TokenType type, object value)
		{
			Type = type;
			Value = value;
		}

		public override bool Equals(object obj)
		{
			var o = obj as Token;

			if (o == null)
				return false;

			return o.Type == Type && o.Value.Equals(Value);
		}

		public bool Equals(Token other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Type == other.Type && Equals(Value, other.Value);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int)Type * 397) ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}

		public static bool operator ==(Token left, Token right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Token left, Token right)
		{
			return !Equals(left, right);
		}
	}
}
