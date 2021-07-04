namespace Loonge.Api.Lexing
{
	public enum Operator
	{
		// Single symbol
		Plus,
		Minus,
		Divide,
		Multiply,
		Modulo,
		BitXor,
		BitOr,
		BitAnd,
		BitNot,
		Assign,
		TernaryFirst,
		TernarySecond,
		Greater,
		Less,
		StringInterpolation,
		LogicalNot,
		StringAsIs,
		Empty,
		MemberAccess,
		// Multiply symbols
		ModuleProvider,
		Lambda,
		Next,
		Equals,
		NotEquals,
		GreaterOrEquals,
		LessOrEquals,
		PlusAssign,
		MinusAssign,
		MultiplyAssign,
		DivideAssign,
		ModuloAssign,
		BitXorAssign,
		BitOrAssign,
		BitAndAssign,
		BitShiftRight,
		BitShiftLeft,
		LogicalAnd,
		LogicalOr,
		Increment,
		Decrement
	}
}
