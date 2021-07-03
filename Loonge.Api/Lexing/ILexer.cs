using System;

namespace Loonge.Api.Lexing
{
	public interface ILexer
	{
		Token Read();
		Token Peek();
		void ThrowException(Exception exception);
	}
}
