using System;
using System.IO;
using loonge.Lexing;

namespace loonge.IO
{
	public sealed class InputStream : IDisposable
	{
		/// <summary>
		/// Gets current line within the stream
		/// </summary>
		public int Line { get; private set; }

		/// <summary>
		/// Gets current column within the stream
		/// </summary>
		public int Column { get; private set; }

		private readonly StringReader _reader;

		public bool IsEndOfStream => Peek() == 0xFFFF || Peek() == -1; // EOF symbol

		public StringReader GetAsStringReader()
		{
			return _reader;
		}

		public InputStream(string filename)
			: this(new StreamReader(filename))
		{ }

		public InputStream(Stream stream)
			: this(new StreamReader(stream))
		{ }

		public InputStream(StreamReader streamReader)
		{
			var content = streamReader.ReadToEnd();
			Line = 1;
			Column = 0;
			streamReader.Close();
			_reader = new StringReader(content);
		}

		public InputStream(StringReader stringReader)
		{
			_reader = stringReader;
			Line = 1;
			Column = 0;
		}

		/// <summary>
        /// Reads and returns the next character AND moves to the next character.
        /// </summary>
		/// <returns>Character</returns>
		public char Read()
		{
			if (IsEndOfStream) throw new EndOfStreamException();

			var ch = (char)_reader.Read();

			if (ch == '\n')
			{
				Line++;
				Column = 0;
			}
			else if (ch == '\r')
			{
				ch = (char)_reader.Peek();

				if (ch == '\n')
				{
					Line++;
					Column = 0;
					ch = (char)_reader.Read();
				}
			}
			else
			{
				Column++;
			}

			return ch;
		}

        /// <summary>
        /// Reads and returns the next character without moving to the next character.
        /// </summary>
        /// <returns>Character</returns>
        public char Peek()
		{
			return (char)_reader.Peek();
		}

		public void Dispose()
		{
			_reader.Dispose();
		}
	}
}
