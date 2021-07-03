namespace Loonge.Lexing
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
		UnaryFirst,
		UnarySecond,
		Greater,
		Less,
		StringInterpolation,
		LogicalNot,
		StringAsIs,
		Empty,
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
		ModuleAssign,
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
