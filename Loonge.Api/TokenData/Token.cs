namespace Loonge.Api.TokenData
{
	public class Token
	{
		public TokenType Type { get; }
		public object Value { get; }

		public Token(TokenType type, object value)
		{
			Type = type;
			Value = value;
		}
	}
}
