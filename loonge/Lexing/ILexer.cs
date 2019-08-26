using System;

namespace loonge.Lexing
{
	public interface ILexer
	{
		Token Read();
		Token Peek();
		void ThrowException(Exception exception);
	}
}
