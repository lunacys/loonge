using System;
using System.IO;
using Loonge.Api;
using Loonge.Api.Exceptions;
using Loonge.Api.IO;
using Loonge.Api.TokenData;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Loonge.Tests
{
    public class Lexer_Test
    {
	    private ILexer _lexer;
	    private InputStream _inputStream;

	    private Token _token;

	    private void T() => _token = _lexer.Read();

	    private readonly string[] _availableKeywords = Enum.GetNames(typeof(Keyword));
	    private readonly string[] _availableTypeAliases = Enum.GetNames(typeof(TypeAlias));

	    public static readonly string GeneratedTestCaseFile = "GeneratedTestCase.txt";
	    
	    public static readonly string CommentsTestFile = "LexerCases/Lexer_Test_Comments.txt";
	    public static readonly string CommentsTestFile2 = "LexerCases/Lexer_Test_Comments2.txt";
	    public static readonly string CharsTestFile = "LexerCases/Lexer_Test_Chars.txt";
	    public static readonly string StringsTestFile = "LexerCases/Lexer_Test_Strings.txt";
	    public static readonly string NumbersTestFile = "LexerCases/Lexer_Test_Numbers.txt";
	    public static readonly string OperatorsTestFile = "LexerCases/Lexer_Test_Operators.txt";
	    public static readonly string WordsTestFile = "LexerCases/Lexer_Test_Words.txt";
	    public static readonly string Complete2TestFile = "LexerCases/Lexer_Test_Complete2.txt";

	    private static void GenerateTestCase()
	    {
		    using var sw = new StreamWriter(GeneratedTestCaseFile, false);

		    sw.WriteLine("/* KEYWORDS */");
		    foreach (var kw in Enum.GetNames<Keyword>())
		    {
			    sw.WriteLine(kw.ToLowerInvariant());
		    }

		    sw.WriteLine("/* TYPE ALIASES */");
		    foreach (var ta in Enum.GetNames<TypeAlias>())
		    {
			    sw.WriteLine(ta.ToLowerInvariant());
		    }

		    sw.WriteLine("/* PUNCTUATION */");
		    var punc = ",[](){};";
		    foreach (var ch in punc)
		    {
			    sw.Write(ch);
		    }
		    sw.WriteLine();
		    
		    sw.WriteLine("/* OPERATORS */");
		    foreach (var kv in Lexer.Operators)
		    {
			    sw.WriteLine(kv.Key);
		    }
	    }
	    
	    public Lexer_Test()
	    {
		    GenerateTestCase();
	    }
	    
        [SetUp]
        public void Setup()
        {
			_inputStream = new InputStream("LexerCases/Lexer_Test_Complete.txt");
			_lexer = new Lexer(_inputStream);
        }

        [Test]
        public void GeneratedCasesTest()
        {
	        using var input = new InputStream(GeneratedTestCaseFile);
	        var lexer = new Lexer(input);
	        
	        foreach (var kw in Enum.GetNames<Keyword>())
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.Keyword, tok.Type);
		        Assert.AreEqual(Enum.Parse<Keyword>(kw), tok.Value);
	        }
	        
	        foreach (var ta in Enum.GetNames<TypeAlias>())
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.TypeAlias, tok.Type);
		        Assert.AreEqual(Enum.Parse<TypeAlias>(ta), tok.Value);
	        }
	        
	        var punc = ",[](){};";
	        foreach (var p in punc)
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.Punctuation, tok.Type);
		        Assert.AreEqual(p, tok.Value);
	        }

	        foreach (var kv in Lexer.Operators)
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.Operator, tok.Type);
		        Assert.AreEqual(kv.Value, tok.Value);
	        }
        }

        [Test]
        public void CommentsTest()
        {
	        using var input = new InputStream(CommentsTestFile);
	        var lexer = new Lexer(input);

	        var checkTokenAndLine = CreateTestAction<Operator>(lexer, TokenType.Operator);
	      

	        var checkEof = new Action(() =>
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(TokenType.Eof, tok.Type, "Token Type");
	        });

	        checkTokenAndLine(Operator.Divide, 1, 1);
	        checkTokenAndLine(Operator.Divide, 14, 1);
	        checkTokenAndLine(Operator.Divide, 17, 1);
	        checkTokenAndLine(Operator.Divide, 20, 1);
	         
	        checkTokenAndLine(Operator.Divide, 23, 1);
	        checkTokenAndLine(Operator.Divide, 23, 3);
	        checkTokenAndLine(Operator.Divide, 23, 5);
	        checkTokenAndLine(Operator.Divide, 24, 1);
	        checkTokenAndLine(Operator.Divide, 24, 3);
	        checkTokenAndLine(Operator.Divide, 25, 1);
	        
	        checkTokenAndLine(Operator.Divide, 25, 28);
	        checkTokenAndLine(Operator.Divide, 25, 30);
	        checkTokenAndLine(Operator.Divide, 25, 32);
	        checkTokenAndLine(Operator.Divide, 25, 34);

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

	        var checkChar = CreateTestAction<char>(lexer, TokenType.Character);

	        checkChar('a', 1, 3);
	        checkChar('b', 1, 7);
	        checkChar('c', 1, 11);
	        checkChar('d', 2, 3);
	        checkChar('e', 2, 7);
	        checkChar('f', 2, 11);
	        checkChar('g', 3, 3);
	        checkChar('h', 3, 6);
	        checkChar('\n', 4, 4);
	        checkChar('\r', 4, 8);

	        Assert.Throws<SyntaxException>(() => lexer.Read());
        }

        [Test]
        public void StringsTest()
        {
	        using var input = new InputStream(StringsTestFile);
	        var lexer = new Lexer(input);

	        var checkString = CreateTestAction<string>(lexer, TokenType.String);

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
        }

        [Test]
        public void NumbersTest()
        {
	        using var input = new InputStream(NumbersTestFile);
	        var lexer = new Lexer(input);
	        
	        var checkInt = CreateTestAction<int>(lexer, TokenType.Number);
	        var checkDouble = CreateTestAction<double>(lexer, TokenType.DecimalNumber);

	        checkInt(123123, 1, 6);
	        checkDouble(123.123, 2, 7);
	        checkDouble(.123, 3, 4);

	        Assert.Throws<SyntaxException>(() => lexer.Read());

	        checkInt(11, 4, 8); // Special for current test
	        checkInt(123123123, 5, 9);
	        
	        Assert.Throws<OverflowException>(() => lexer.Read());
        }

        [Test]
        public void OperatorsTest()
        {
	        using var input = new InputStream(OperatorsTestFile);
	        var lexer = new Lexer(input);
	        
	        var checkOp = CreateTestAction<Operator>(lexer, TokenType.Operator);

	        checkOp(Operator.Plus, 1, 1);
	        checkOp(Operator.Minus, 2, 1);
	        checkOp(Operator.Equals, 3, 2);
	        checkOp(Operator.NotEquals, 4, 2);
	        
	        checkOp(Operator.LogicalAnd, 5, 2);
	        checkOp(Operator.LogicalOr, 5, 4);
	        
	        checkOp(Operator.Lambda, 6, 2);
	        checkOp(Operator.ModuloAssign, 6, 5);
	        
	        checkOp(Operator.BitXorAssign, 7, 2);
	        checkOp(Operator.BitOrAssign, 7, 5);
        }

        [Test]
        public void WordsTest()
        {
	        using var input = new InputStream(WordsTestFile);
	        var lexer = new Lexer(input);

	        var checkType = CreateTestAction<TypeAlias>(lexer, TokenType.TypeAlias);
	        var checkKeyword = CreateTestAction<Keyword>(lexer, TokenType.Keyword);
	        var checkIdent = CreateTestAction<string>(lexer, TokenType.Identifier);
	        var checkOp = CreateTestAction<Operator>(lexer, TokenType.Operator);
	        
	        checkKeyword(Keyword.Fn, -1, -1);
	        checkKeyword(Keyword.Let, -1, -1);
	        checkKeyword(Keyword.If, -1, -1);
	        checkKeyword(Keyword.Else, -1, -1);
	        checkKeyword(Keyword.Type, -1, -1);

	        checkType(TypeAlias.I32, -1, -1);
	        checkType(TypeAlias.I64, -1, -1);
	        checkType(TypeAlias.F32, -1, -1);
	        checkType(TypeAlias.F64, -1, -1);

	        checkIdent("test1", -1, -1);
	        checkIdent("test12", -1, -1);
	        checkIdent("test13", -1, -1);
	        checkOp(Operator.MemberAccess, -1, -1);
	        checkIdent("test14", -1, -1);
	        checkIdent("test15", -1, -1);
        }

        [Test]
        public void Complete2Test()
        {
	        using var input = new InputStream(Complete2TestFile);
	        var lexer = new Lexer(input);

	        var checkKeyword = CreateTestActionNoLineCheck<Keyword>(lexer, TokenType.Keyword);
	        var checkIdent = CreateTestActionNoLineCheck<string>(lexer, TokenType.Identifier);
	        var checkOp = CreateTestActionNoLineCheck<Operator>(lexer, TokenType.Operator);
	        var checkPunc = CreateTestActionNoLineCheck<char>(lexer, TokenType.Punctuation);
	        var checkType = CreateTestActionNoLineCheck<TypeAlias>(lexer, TokenType.TypeAlias);
	        var checkNum = CreateTestActionNoLineCheck<int>(lexer, TokenType.Number);
	        var checkNumD = CreateTestActionNoLineCheck<double>(lexer, TokenType.DecimalNumber);
	        
	        // let std = import std;
	        checkKeyword(Keyword.Let);
	        checkIdent("std");
	        checkOp(Operator.Assign);
	        checkKeyword(Keyword.Import);
	        checkIdent("std");
	        checkPunc(';');
	        
	        // let dbg = import std.debug;
	        checkKeyword(Keyword.Let);
	        checkIdent("dbg");
	        checkOp(Operator.Assign);
	        checkKeyword(Keyword.Import);
	        checkIdent("std");
	        checkOp(Operator.MemberAccess);
	        checkIdent("debug");
	        checkPunc(';');

	        // export module lexerTest {
	        checkKeyword(Keyword.Export);
	        checkKeyword(Keyword.Module);
	        checkIdent("lexerTest");
	        checkPunc('{');
	        
	        // pub struct point {
	        checkKeyword(Keyword.Pub);
	        checkKeyword(Keyword.Struct);
	        checkIdent("point");
	        checkPunc('{');
	        
	        // pub mut x: i32;
	        checkKeyword(Keyword.Pub);
	        checkKeyword(Keyword.Mut);
	        checkIdent("x");
	        checkOp(Operator.TypeAssign);
	        checkType(TypeAlias.I32);
	        checkPunc(';');
	        
	        // pub mut y: i32;
	        checkKeyword(Keyword.Pub);
	        checkKeyword(Keyword.Mut);
	        checkIdent("y");
	        checkOp(Operator.TypeAssign);
	        checkType(TypeAlias.I32);
	        checkPunc(';');
	        
	        // } 
	        checkPunc('}');
	        
	        // } 
	        checkPunc('}');
	        
	        // let mut x = 15;
	        checkKeyword(Keyword.Let);
	        checkKeyword(Keyword.Mut);
	        checkIdent("x");
	        checkOp(Operator.Assign);
	        checkNum(15);
	        checkPunc(';');
	        
	        // let mut y = 10;
	        checkKeyword(Keyword.Let);
	        checkKeyword(Keyword.Mut);
	        checkIdent("y");
	        checkOp(Operator.Assign);
	        checkNum(10);
	        checkPunc(';');
	        
	        // x += 5;
	        checkIdent("x");
	        checkOp(Operator.PlusAssign);
	        checkNum(5);
	        checkPunc(';');

	        // y += 10;
	        checkIdent("y");
	        checkOp(Operator.PlusAssign);
	        checkNum(10);
	        checkPunc(';');
	        
	        // let xD = 15.1;
	        checkKeyword(Keyword.Let);
	        checkIdent("xD");
	        checkOp(Operator.Assign);
	        checkNumD(15.1);
	        checkPunc(';');
	        
	        // let yD = 15.51;
	        checkKeyword(Keyword.Let);
	        checkIdent("yD");
	        checkOp(Operator.Assign);
	        checkNumD(15.51);
	        checkPunc(';');
	        
	        // let a = .15;
	        checkKeyword(Keyword.Let);
	        checkIdent("a");
	        checkOp(Operator.Assign);
	        checkNumD(.15);
	        checkPunc(';');
	        
	        // dbg.assert.areEqual(x, y);
	        checkIdent("dbg");
	        checkOp(Operator.MemberAccess);
	        checkIdent("assert");
	        checkOp(Operator.MemberAccess);
	        checkIdent("areEqual");
	        checkPunc('(');
	        checkIdent("x");
	        checkPunc(',');
	        checkIdent("y");
	        checkPunc(')');
	        checkPunc(';');
        }

        [Test]
        public void General()
        {
	        //Assert.Pass();
	        
	        //foreach (var keyword in _availableKeywords)
		    //    NextAndCheckKeyword(keyword);

	        foreach (var typeAlias in _availableTypeAliases)
		        NextAndCheckTypeAlias(typeAlias);


	        CheckOperators(
		        "+", "-", "/", "*", "%", "^", "|", "&", "~", "=", "?", ":", ">", "<", "$", "!", "@",
		        "::", "=>", "->", "==", "!=", ">=", "<=", "+=", "-=", "*=", "/=", "%=", "^=", "|=", "&=", ">>", "<<",
		        "&&", "||", "++", "--"
	        );
	        CheckDelimiters(',', '[', ']', '(', ')', '{', '}', ';');
			CheckStrings(
				"string1", "string1.1", 
				"string2", "string2.1", "string2.2", 
				"string3", 
				"not closed string", 
				"   ");
			CheckChars('a', ' ', '\n');
        }

		[Test]
        public void ExceptionsTest()
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

        private static Action<T, int, int> CreateTestAction<T>(Lexer lexer, TokenType expectedTokenType)
        {
	        return (p1, line, column) =>
	        {
		        var tok = lexer.Read();
		        Assert.AreEqual(expectedTokenType, tok.Type, "Token Type");
		        Assert.AreEqual(p1, (T) tok.Value, "Token Value");
		        if (line != -1 && column != -1)
		        {
			        Assert.AreEqual(line, lexer.Line, $"Invalid Line (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
			        Assert.AreEqual(column, lexer.Column, $"Invalid Column (exp: {line}:{column}, got: {lexer.Line}:{lexer.Column})");
		        }
	        };
        }
        
        private static Action<T> CreateTestActionNoLineCheck<T>(Lexer lexer, TokenType expectedTokenType)
        {
	        return (p1) =>
	        {
		        Token tok = null;
		        Assert.DoesNotThrow(() =>
		        {
			        tok = lexer.Read();
		        }, $"({lexer.Line}:{lexer.Column})");
		        Assert.AreEqual(expectedTokenType, tok.Type, $"Token Type (value exp: {p1}, got: {tok.Value})");
		        Assert.AreEqual(p1, (T) tok.Value, "Token Value");
	        };
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
	        Assert.AreEqual(TokenType.Keyword, _token.Type, $"Expected Keyword (Token Value: {_token.Value})");
	        Assert.That(Enum.TryParse(_token.Value.ToString(), true, out kw), $"Expected keyword {keyword}, but got {kw}");
			Assert.AreEqual(keyword, kw);
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

        private void CheckOperators(params string[] ops)
        {
	        foreach (var op in ops)
	        {
		        T();
		        Assert.AreEqual(TokenType.Operator, _token.Type, $"({_lexer.Line}:{_lexer.Column})");
		        Assert.AreEqual(Lexer.Operators[op], (Operator)_token.Value, $"({_lexer.Line}:{_lexer.Column})");
	        }
        }

        private void CheckDelimiter(char expected)
        {
	        T();
			Assert.AreEqual(TokenType.Punctuation, _token.Type, $"Expected Punctuation (Token Value: {_token.Value}) ({_lexer.Line}:{_lexer.Column})");
			Assert.DoesNotThrow(() =>
			{
				var test = (char)_token.Value;
			});
			Assert.AreEqual(expected, (char)_token.Value);
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