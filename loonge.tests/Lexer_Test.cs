using loonge.IO;
using loonge.Lexing;
using NUnit.Framework;

namespace loonge.tests
{
    public class Lexer_Test
    {
	    private Lexer _lexer;
	    private InputStream _inputStream;

	    private Token _token;

	    private void T() => _token = _lexer.Read();

        [SetUp]
        public void Setup()
        {
			_inputStream = new InputStream("Lexer_Test1.txt");
			_lexer = new Lexer(_inputStream);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

		[TearDown]
        public void CleanUp()
        {
			_inputStream.Dispose();
        }
    }
}