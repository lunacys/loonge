﻿namespace Loonge.Api.TokenData
{
	public enum TokenType
	{
		/// <summary>
        /// End Of File (Stream)
        /// </summary>
		Eof = -1,
		Punctuation = 0,
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
