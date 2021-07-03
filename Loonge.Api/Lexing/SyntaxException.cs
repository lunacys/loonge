using System;

namespace Loonge.Api.Lexing
{
    public class SyntaxException : Exception 
    {
        public int Position { get; }
        public int Line { get; }
        public int Column { get; }
        
        public SyntaxException(string message)
            : base(message)
        {
            
        }

        public SyntaxException(string message, int position, int line, int column)
            : base(message + $" ({line}:{column})")
        {
            Position = position;
            Line = line;
            Column = column;
        }
    }
}