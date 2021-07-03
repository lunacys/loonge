using System;

namespace Loonge.Lexing
{
	public interface ILexer
	{
		Token Read();
		Token Peek();
		void ThrowException(Exception exception);
	}
}
