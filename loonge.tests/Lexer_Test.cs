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
        public void General()
        {
	        Assert.Pass("Success");
        }

		[Test]
        public void TestForExceptions()
        {
	        Assert.DoesNotThrow(() => _lexer.Peek(), "Peek() method must not throw an exception");
	        Assert.DoesNotThrow(() => _lexer.Read(), "Read() method must not throw an exception");
	        Assert.Throws<InvalidTokenException>(() => _lexer.ThrowException(new InvalidTokenException(1, 0)),
		        "ThrowException() method must throw exception of type InvalidTokenException");
        }

		[TearDown]
        public void CleanUp()
        {
			_inputStream.Dispose();
        }
    }
}