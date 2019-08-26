using loonge.IO;
using NUnit.Framework;

namespace loonge.tests
{
	[TestFixture]
	public class InputStream_Test
	{
		[SetUp]
		public void SetUp()
		{

		}

		[Test]
		[TestCase("InputStream_Test1.txt")]
		public void Common(string filename)
		{
			using (InputStream input = new InputStream(filename))
			{
				Assert.That(input.Column == 0 && input.Line == 1);

				for (int i = 1; i <= 6; i++)
				{
					var ch = input.Read();
					var chAsInt = int.Parse(ch.ToString());

					Assert.That(chAsInt == i, $"Error: i = {i}, chAsInt = {chAsInt}, ch = {ch}");
					Assert.That(input.Line == 1);
					Assert.That(input.Column == i, $"Error: input.Column = {input.Column}, i = {i}");
				}

				var newLineChar = input.Read();

				Assert.That(newLineChar == '\n' || newLineChar == '\r', $"Error: new line expected, but got: {newLineChar}");

				if (newLineChar == '\r')
				{
					newLineChar = input.Read();
					Assert.That(newLineChar == '\n', $"Error: expected new line char after \\r, but got: {newLineChar}");
				}

				Assert.That(input.Line == 2, $"Error: input.Line must be 2, but got: {input.Line}");
				Assert.That(input.Column == 0, $"Error: input.Column must be 0, but got: {input.Column}");

				while (!input.IsEndOfStream)
				{
					var ch = input.Read();

					Assert.That(char.IsDigit(ch) || ch == '\r' || ch == '\n', $"Error: got an invalid char: {ch}");
				}
			}
        }
	}
}
