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
		void ThrowException(Exception exception);
	}
}
