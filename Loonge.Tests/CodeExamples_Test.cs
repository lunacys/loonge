using System;
using System.Collections.Generic;
using Loonge.Api;
using Loonge.Api.IO;
using Loonge.Api.TokenData;
using NUnit.Framework;

namespace Loonge.Tests
{
    public class CodeExamples_Test
    {
        private InputStream _input;
        private ILexer _lexer;
        private IParser _parser;

        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void CleanUp()
        {
            if (_input != null)
                _input.Dispose();
        }

        [Test]
        [TestCase("CodeExamples/Level1.ln")]
        [TestCase("CodeExamples/Level2.ln")]
        public void LexerTokenSanityTest(string fileName)
        {
            InitForFile(fileName);

            Assert.DoesNotThrow(() =>
            {
                while (_lexer.Read().Type != TokenType.Eof) ;
            });
        }

        private void InitForFile(string filename)
        {
            _input = new InputStream(filename);
            _lexer = new Lexer(_input);
            _parser = null;
        }
    }
}