using Loonge.Api.TokenData;

namespace Loonge.Api
{
    public interface IParser
    {
        void Parse(Token token);
    }
}