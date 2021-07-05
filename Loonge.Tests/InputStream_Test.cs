using System;
using System.IO;
using System.Linq;
using Loonge.Api.IO;
using NUnit.Framework;

namespace Loonge.Tests
{
	[TestFixture]
	public class InputStream_Test
	{
		[SetUp]
		public void SetUp()
		{ }

		[Test]
        public void SanityTest()
        {
            var input = new InputStream("InputStream_Test1.txt");

            StringReader sr = null;
			Assert.DoesNotThrow(() => sr = input.ToStringReader());

			Assert.IsNotNull(sr);
			Assert.DoesNotThrow(() =>
            {
                sr.Peek();
                sr.Read();
            });

			Assert.DoesNotThrow(() =>
            {
                input.Dispose();
			});

			Assert.DoesNotThrow(() => sr = input.ToStringReader());
			Assert.Throws<ObjectDisposedException>(() => input.Read());

            input = new InputStream(new StreamReader("InputStream_Test1.txt"));
            sr = null;
            Assert.DoesNotThrow(() => sr = input.ToStringReader());

            Assert.IsNotNull(sr);
            Assert.DoesNotThrow(() =>
            {
                sr.Peek();
                sr.Read();
            });

            Assert.DoesNotThrow(() =>
            {
                input.Dispose();
            });

            Assert.DoesNotThrow(() => sr = input.ToStringReader());
            Assert.Throws<ObjectDisposedException>(() => input.Read());
		}

		[Test]
		[TestCase("InputStream_Test1.txt")]
		public void LoadFromFileTest1(string filename)
		{
			using InputStream input = new InputStream(filename);
			
			TestInputStreamTest1(input);

			var expectedPosition2 = GetTextSize(filename);
			Assert.AreEqual(expectedPosition2.pos, input.Position);
			Assert.AreEqual(expectedPosition2.line, input.Line);
			Assert.AreEqual(expectedPosition2.column, input.Column);

			Assert.Throws<EndOfStreamException>(() => input.Read());
		}
		
		[Test]
		[TestCase("123456\n123\n12345\n123\n12345678")]
		public void LoadFromStringContentTest1(string content)
		{
			using InputStream input = new InputStream(content, false);
			
			TestInputStreamTest1(input);
			
			var expectedPosition2 = GetTextSize(content, false);
			Assert.AreEqual(expectedPosition2.pos, input.Position);
			Assert.AreEqual(expectedPosition2.line, input.Line);
			Assert.AreEqual(expectedPosition2.column, input.Column);
		}

		private void TestInputStreamTest1(InputStream input)
		{
			Assert.That(input.Column == 0 && input.Line == 1 && input.Position == 0);

			for (int i = 1; i <= 6; i++)
            {
                char ch = ' ';
                int chAsInt = 0;
				Assert.DoesNotThrow(() => ch = input.Read());
				Assert.DoesNotThrow(() => chAsInt = int.Parse(ch.ToString()));

				Assert.That(chAsInt == i, $"Error: i = {i}, chAsInt = {chAsInt}, ch = {ch}");
				Assert.That(input.Line == 1);
				Assert.That(input.Column == i, $"Error: input.Column = {input.Column}, i = {i}");
			}
			
			Assert.AreEqual(input.Position, 6);
			Assert.AreEqual(input.Line, 1);
			Assert.AreEqual(input.Column, 6);

            char newLineChar = ' ';
            Assert.DoesNotThrow(() => newLineChar = input.Read());

            Assert.That(newLineChar == '\n' || newLineChar == '\r', $"Error: new line expected, but got: {newLineChar}");

			var expectedPosition = 7;
			
			// Special for \r\n
			if (newLineChar == '\r')
			{
				newLineChar = input.Read();
				Assert.That(newLineChar == '\n', $"Error: expected new line char after \\r, but got: {newLineChar}");

				expectedPosition++;
			}

			Assert.That(input.Line == 2, $"Error: input.Line must be 2, but got: {input.Line}");
			Assert.That(input.Column == 0, $"Error: input.Column must be 0, but got: {input.Column}");
			Assert.AreEqual(input.Position, expectedPosition);

			while (!input.IsEndOfStream)
            {
                char ch = ' ';
				Assert.DoesNotThrow(() => ch = input.Read());

                Assert.That(char.IsDigit(ch) || ch == '\r' || ch == '\n', $"Error: got an invalid char: {ch}. Expected either \\r or \\n or digit");
			}
			
			Assert.Throws<EndOfStreamException>(() => input.Read());
		}
		
		private (int pos, int line, int column) GetTextSize(string filename, bool isFile = true)
		{
			string allText;
			
			if (isFile)
			{
				using var sr = new StreamReader(filename);
				allText = sr.ReadToEnd();
			}
			else
			{
				allText = filename;
			}
			
			var lines = allText.Contains('\r') ? allText.Split("\r\n") : allText.Split('\n');
			var column = lines.Last().Length;

			return (allText.Replace("\r", "").Length, lines.Length, column);
		}
	}
}
