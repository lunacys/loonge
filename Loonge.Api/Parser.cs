using Loonge.Api.TokenData;

namespace Loonge.Api
{
    public class Parser : IParser
    {
        private ILexer _lexer;

        public Parser(ILexer lexer)
        {
            _lexer = lexer;
        }

        public void Parse(Token token)
        {
            throw new System.NotImplementedException();
        }
    }
}