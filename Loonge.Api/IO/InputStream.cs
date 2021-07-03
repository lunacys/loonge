using System;
using System.Diagnostics;
using System.IO;

namespace Loonge.Api.IO
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
		
		/// <summary>
		/// Gets current cursor position
		/// </summary>
		public int Position { get; private set; }
		
		public int TotalSize { get; private set; }

		private StringReader _reader;

		public bool IsEndOfStream => Peek() == 0xFFFF; // EOF symbol

		public StringReader ToStringReader()
		{
			return _reader;
		}

		public InputStream(string filenameOrContent, bool fromFile = true)
		{
			if (fromFile)
			{
				InitializeFromFile(new StreamReader(filenameOrContent));
			}
			else
			{
				InitializeFromString(filenameOrContent);
			}
		}

		public InputStream(Stream stream)
			: this(new StreamReader(stream))
		{ }

		public InputStream(StreamReader streamReader)
		{
			InitializeFromFile(streamReader);
		}

		/// <summary>
        /// Reads and returns the next character AND moves to the next character.
        /// </summary>
		/// <returns>Character</returns>
		public char Read()
		{
			if (IsEndOfStream) throw new EndOfStreamException();

			var ch = (char)_reader.Read();

			Position++;

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
        
        private void Dispose(bool disposing)
        {
	        if (disposing)
	        {
		        _reader.Dispose();
	        }
        }

        private void InitializeFromFile(StreamReader streamReader)
        {
	        var content = streamReader.ReadToEnd();
	        Line = 1;
	        Column = 0;
	        Position = 0;
	        streamReader.Close();
	        TotalSize = content.Length;

	        _reader = new StringReader(content);
        }

        private void InitializeFromString(string content)
        {
	        Line = 1;
	        Column = 0;
	        Position = 0;
	        TotalSize = content.Length;

	        _reader = new StringReader(content);
        }

        public void Dispose()
        {
	        Dispose(true);
	        GC.SuppressFinalize(this);
        }

        ~InputStream()
        {
	        Dispose(false);
        }
	}
}
