using System;
using Loonge.Api.TokenData;

namespace Loonge.Api
{
	public interface ILexer
	{
		int Position { get; }
		int Line { get; }
		int Column { get; }

        Token Read();
		Token Peek();
		Exception GetException(string additionalMessage = null, Exception innerException = null);
        void ThrowException(string additionalMessage = null, Exception innerException = null);
    }
}
