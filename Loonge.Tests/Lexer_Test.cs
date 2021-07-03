using System;
using Loonge.Api.IO;
using Loonge.Api.Lexing;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Loonge.Tests
{
    public class Lexer_Test
    {
	    private Lexer _lexer;
	    private InputStream _inputStream;

	    private Token _token;

	    private void T() => _token = _lexer.Read();

	    private readonly string[] _availableKeywords = Enum.GetNames(typeof(Keyword));
	    private readonly string[] _availableTypeAliases = Enum.GetNames(typeof(TypeAlias));

	    public static readonly string CommentsTestFile = "LexerCases/Lexer_Test_Comments.txt";
	    public static readonly string CommentsTestFile2 = "LexerCases/Lexer_Test_Comments2.txt";
	    public static readonly string CharsTestFile = "LexerCases/Lexer_Test_Chars.txt";
	    public static readonly string StringsTestFile = "LexerCases/Lexer_Test_Strings.txt";
	    public static readonly string NumbersTestFile = "LexerCases/Lexer_Test_Numbers.txt";
	    public static readonly string OperatorsTestFile = "LexerCases/Lexer_Test_Operators.txt";

        [SetUp]
        public void Setup()
        {
			_inputStream = new InputStream("LexerCases/Lexer_Test_Complete.txt");
			_lexer = new Lexer(_inputStream);
        }

        [Test]
        public void CommentsTest()
        {
	        using var input = new InputStream(CommentsTestFile);
	        var lexer = new Lexer(input);

	        var checkTokenAndLine = new Action<int, int>((line, column) =>
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.Operator, tok.Type, "Token Type");
		        Assert.AreEqual(Operator.Divide, (Operator) tok.Value, "Token Value");
		        if (line != -1 && column != -1)
		        {
			        Assert.AreEqual(line, lexer.Line, $"Invalid Line (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
			        Assert.AreEqual(column, lexer.Column, $"Invalid Column (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
		        }
	        });

	        var checkEof = new Action(() =>
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.Eof, tok.Type, "Token Type");
	        });

	        checkTokenAndLine(1, 1);
	        checkTokenAndLine(14, 1);
	        checkTokenAndLine(17, 1);
	        checkTokenAndLine(20, 1);
	        
	        checkTokenAndLine(23, 1);
	        checkTokenAndLine(23, 3);
	        checkTokenAndLine(23, 5);
	        checkTokenAndLine(24, 1);
	        checkTokenAndLine(24, 3);
	        checkTokenAndLine(25, 1);
	        
	        checkTokenAndLine(25, 28);
	        checkTokenAndLine(25, 30);
	        checkTokenAndLine(25, 32);
	        checkTokenAndLine(25, 34);

	        checkEof();
        }

        [Test]
        public void CommentsTests2()
        {
	        using var input = new InputStream(CommentsTestFile2);
	        var lexer = new Lexer(input);

	        var tok = lexer.Read();
	        Assert.AreEqual(tok.Type, TokenType.Eof);
        }

        [Test]
        public void CharsTests()
        {
	        using var input = new InputStream(CharsTestFile);
	        var lexer = new Lexer(input);

	        var checkChar = new Action<char, int, int>((ch, line, column) =>
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.Character, tok.Type, "Token Type");
		        Assert.AreEqual(ch, (char) tok.Value, "Token Value");
		        if (line != -1 && column != -1)
		        {
			        Assert.AreEqual(line, lexer.Line, $"Invalid Line (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
			        Assert.AreEqual(column, lexer.Column, $"Invalid Column (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
		        }
	        });

	        checkChar('a', 1, 3);
	        checkChar('b', 1, 7);
	        checkChar('c', 1, 11);
	        checkChar('d', 2, 3);
	        checkChar('e', 2, 7);
	        checkChar('f', 2, 11);
	        checkChar('g', 3, 3);
	        checkChar('h', 3, 6);

	        Assert.Throws<SyntaxException>(() => lexer.Read());
        }

        [Test]
        public void StringsTest()
        {
	        using var input = new InputStream(StringsTestFile);
	        var lexer = new Lexer(input);

	        var checkString = new Action<string, int, int>((str, line, column) =>
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.String, tok.Type, "Token Type");
		        Assert.AreEqual(str, (string) tok.Value, "Token Value");
		        if (line != -1 && column != -1)
		        {
			        Assert.AreEqual(line, lexer.Line, $"Invalid Line (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
			        Assert.AreEqual(column, lexer.Column, $"Invalid Column (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
		        }
	        });

	        checkString("test string!!!", 1, 16);
	        checkString("", 2, 2);
	        checkString("/n/r/n", 3, 8);
	        checkString("test1", 3, 15);
	        checkString("test2", 3, 22);
	        checkString("\r\n\t\'", 4, 10);
	        checkString("double \" here", 5, 16);
	        checkString("\"double quote\"", 6, 18);
	        checkString("\n\t\r\'\"\\\0\a\b\v\f", 7, 24);
	        
	        Assert.Throws<SyntaxException>(() => lexer.Read());
	        Assert.Throws<SyntaxException>(() => lexer.Read());
        }

        [Test]
        public void NumbersTest()
        {
	        using var input = new InputStream(NumbersTestFile);
	        var lexer = new Lexer(input);

	        var checkInt = new Action<int, int, int>((num, line, column) =>
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.Number, tok.Type, "Token Type");
		        Assert.AreEqual(num, (int) tok.Value, "Token Value");
		        if (line != -1 && column != -1)
		        {
			        Assert.AreEqual(line, lexer.Line, $"Invalid Line (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
			        Assert.AreEqual(column, lexer.Column, $"Invalid Column (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
		        }
	        });
	        
	        var checkDouble = new Action<double, int, int>((num, line, column) =>
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.DecimalNumber, tok.Type, "Token Type");
		        Assert.AreEqual(num, (double) tok.Value, "Token Value");
		        if (line != -1 && column != -1)
		        {
			        Assert.AreEqual(line, lexer.Line, $"Invalid Line (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
			        Assert.AreEqual(column, lexer.Column, $"Invalid Column (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
		        }
	        });

	        checkInt(123123, 1, 7);
	        checkDouble(123.123, 2, 8);
	        checkDouble(.123, 3, 5);

	        Assert.Throws<SyntaxException>(() => lexer.Read());

	        checkInt(11, 4, 9); // Special for current test
	        checkInt(123123123, 5, 10);
	        
	        Assert.Throws<OverflowException>(() => lexer.Read());
        }

        [Test]
        public void OperatorsTest()
        {
	        using var input = new InputStream(OperatorsTestFile);
	        var lexer = new Lexer(input);

	        var checkOp = new Action<Operator, int, int>((op, line, column) =>
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.Operator, tok.Type, "Token Type");
		        Assert.AreEqual(op, (Operator) tok.Value, "Token Value");
		        if (line != -1 && column != -1)
		        {
			        Assert.AreEqual(line, lexer.Line, $"Invalid Line (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
			        Assert.AreEqual(column, lexer.Column, $"Invalid Column (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
		        }
	        });

	        checkOp(Operator.Plus, 1, 1);
	        checkOp(Operator.Minus, 2, 1);
	        checkOp(Operator.Equals, 3, 2);
	        checkOp(Operator.NotEquals, 4, 2);
	        
	        checkOp(Operator.LogicalAnd, 5, 2);
	        checkOp(Operator.LogicalOr, 5, 4);
	        
	        checkOp(Operator.Lambda, 6, 2);
	        checkOp(Operator.ModuleAssign, 6, 5);
	        
	        checkOp(Operator.BitXorAssign, 7, 2);
	        checkOp(Operator.BitOrAssign, 7, 5);
        }

        [Test]
        public void General()
        {
	        foreach (var keyword in _availableKeywords)
		        NextAndCheckKeyword(keyword);

	        foreach (var typeAlias in _availableTypeAliases)
		        NextAndCheckTypeAlias(typeAlias);

			CheckDelimiters('.', ',', '[', ']', '(', ')', '{', '}', ';');
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

			Assert.DoesNotThrow(() =>
			{
				while (_lexer.Read().Type != TokenType.Eof) ;
            });
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

        private void CheckDelimiters(params char[] expected)
        {
	        foreach (var delimiter in expected)
	        {
		        CheckDelimiter(delimiter);
	        }
        }

		/// <summary>
        /// 
        /// </summary>
        /// <param name="expected">Pass null if there must be an error/exception</param>
        private void CheckString(string expected)
        {
	        if (expected != null)
	        {
		        Assert.DoesNotThrow(T);
	        }

            Assert.That(_token.Type == TokenType.String, $"Expected String (Token Value: {_token.Value})");

            if (expected == null)
	            return;

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