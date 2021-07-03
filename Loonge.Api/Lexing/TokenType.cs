namespace Loonge.Api.Lexing
{
	public enum TokenType
	{
		/// <summary>
        /// End Of File (Stream)
        /// </summary>
		Eof = -1,
		Delimiter = 0,
		Keyword = 1,
		TypeAlias = 2,
		Operator = 3,
		String = 4,
		Character = 5,
		Number = 6,
		DecimalNumber = 7,
		/// <summary>
		/// Usually name of a variable
		/// </summary>
		Identifier = 8,
	}
}
