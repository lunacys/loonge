using System;
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

	    private readonly string[] _availableKeywords = Enum.GetNames(typeof(Keyword));
	    private readonly string[] _availableTypeAliases = Enum.GetNames(typeof(TypeAlias));

        [SetUp]
        public void Setup()
        {
			_inputStream = new InputStream("Lexer_Test1.txt");
			_lexer = new Lexer(_inputStream);
        }

        [Test]
        public void General()
        {
	        foreach (var keyword in _availableKeywords)
		        NextAndCheckKeyword(keyword);

	        foreach (var typeAlias in _availableTypeAliases)
		        NextAndCheckTypeAlias(typeAlias);

			CheckDelimiter('.');
			CheckDelimiter(',');
			CheckDelimiter('[');
			CheckDelimiter(']');
			CheckDelimiter('(');
			CheckDelimiter(')');
			CheckDelimiter('{');
			CheckDelimiter('}');
			CheckDelimiter(';');

			CheckStrings("string1", "string1.1", "string2", "string2.1", "string2.2", "string3", null, "  ");

			CheckChars('a', ' ', '\n');
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

        private void NextAndCheckKeyword(string keyword)
        {
	        NextAndCheckKeyword(Enum.Parse<Keyword>(keyword, true));
        }

        private void NextAndCheckKeyword(Keyword keyword)
        {
			T();
			CheckKeyword(keyword);
        }

        private void CheckKeyword(Keyword keyword)
        {
	        Keyword kw;
	        Assert.That(_token.Type == TokenType.Keyword, $"Expected Keyword (Token Value: {_token.Value})");
	        Assert.That(Enum.TryParse(_token.Value.ToString(), true, out kw), $"Expected keyword {keyword}, but got {kw}");
			Assert.That(kw == keyword);
        }

        private void NextAndCheckTypeAlias(string typeAlias)
        {
	        NextAndCheckTypeAlias(Enum.Parse<TypeAlias>(typeAlias, true));
        }

        private void NextAndCheckTypeAlias(TypeAlias typeAlias)
        {
	        T();
	        CheckTypeAlias(typeAlias);
        }

        private void CheckTypeAlias(TypeAlias typeAlias)
        {
	        TypeAlias ta;
	        Assert.That(_token.Type == TokenType.TypeAlias, $"Expected TypeAlias (Token Value: {_token.Value})");
	        Assert.That(Enum.TryParse(_token.Value.ToString(), true, out ta), $"Expected type alias {typeAlias}, but got {ta}");
	        Assert.That(ta == typeAlias);
        }

        private void CheckDelimiter(char expected)
        {
	        T();
			Assert.That(_token.Type == TokenType.Delimiter, $"Expected Delimiter (Token Value: {_token.Value})");
			Assert.DoesNotThrow(() =>
			{
				var test = (char)_token.Value;
			});
			Assert.That((char)_token.Value == expected);
        }

		/// <summary>
        /// 
        /// </summary>
        /// <param name="expected">Pass null if there must be an error/exception</param>
        private void CheckString(string expected)
        {
	        if (expected != null)
		        Assert.DoesNotThrow(T);
            Assert.That(_token.Type == TokenType.String, $"Expected String (Token Value: {_token.Value})");
            Assert.DoesNotThrow(() =>
			{
				var test = (string)_token.Value;
			});
			Assert.AreEqual(expected, (string)_token.Value);
        }

		private void CheckStrings(params string[] strs)
		{
			foreach (var str in strs)
			{
				CheckString(str);
			}
		}

        private void CheckChar(char expected)
		{
			T();
			Assert.That(_token.Type == TokenType.Character, $"Expected Character (Token Value: {_token.Value})");
			Assert.DoesNotThrow(() =>
			{
				var test = (char)_token.Value;
			});
			Assert.AreEqual(expected, (char)_token.Value);
		}

        private void CheckChars(params char[] chrs)
        {
	        foreach (var chr in chrs)
	        {
		        CheckChar(chr);
	        }
        }
    }
}