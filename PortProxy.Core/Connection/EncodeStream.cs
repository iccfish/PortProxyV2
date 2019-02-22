namespace PortProxy.Connection
{
	using System;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;

	class EncodeStream : Stream
	{
		private Stream _baseStream;
		private byte? _readKey, _writeKey;
		private static readonly Random _random = new Random();

		public EncodeStream(Stream baseStream)
		{
			_baseStream = baseStream;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		/// <summary>Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream. Instead of calling this method, ensure that the stream is properly disposed.</summary>
		public override void Close()
		{
			_baseStream.Close();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		/// <summary>When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public override void Flush()
		{
			_baseStream.Flush();
		}

		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (_readKey == null)
			{
				var tmp = new byte[1];
				if (await _baseStream.ReadAsync(tmp, 0, 1) != 1)
				{
					return 0;
				}

				_readKey = tmp[0];
			}

			var bytesRead = await _baseStream.ReadAsync(buffer, offset, count, cancellationToken);
			if (bytesRead > 0)
			{
				for (int i = 0; i < bytesRead; i++)
				{
					var index = offset + i;
					buffer[index] ^= _readKey.Value;
					_readKey = buffer[index];
				}
			}

			return bytesRead;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return ReadAsync(buffer, 0, count).Result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin != SeekOrigin.Current)
				offset -= 1;
			return _baseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			if (value < 1)
				throw new ArgumentOutOfRangeException();
			_baseStream.SetLength(value - 1);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			WriteAsync(buffer, offset, count).Wait();
		}

		public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (_writeKey == null)
			{
				_writeKey = (byte)(1 + _random.Next(254));
				await _baseStream.WriteAsync(new[] {_writeKey.Value}, 0, 1);
			}

			for (int i = 0; i < count; i++)
			{
				var index = offset + i;
				var temp = buffer[index];
				buffer[index] ^= _writeKey.Value;
				_writeKey = temp;
			}

			await _baseStream.WriteAsync(buffer, offset, count);
		}

		/// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports reading.</summary>
		/// <returns>true if the stream supports reading; otherwise, false.</returns>
		public override bool CanRead => _baseStream.CanRead;

		/// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports seeking.</summary>
		/// <returns>true if the stream supports seeking; otherwise, false.</returns>
		public override bool CanSeek => _baseStream.CanSeek;

		/// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports writing.</summary>
		/// <returns>true if the stream supports writing; otherwise, false.</returns>
		public override bool CanWrite => _baseStream.CanWrite;

		/// <summary>When overridden in a derived class, gets the length in bytes of the stream.</summary>
		/// <returns>A long value representing the length of the stream in bytes.</returns>
		/// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override long Length => _baseStream.Length + 1;

		/// <summary>When overridden in a derived class, gets or sets the position within the current stream.</summary>
		/// <returns>The current position within the stream.</returns>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support seeking.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override long Position
		{
			get => _baseStream.Position + 1;
			set => _baseStream.Position = value - 1;
		}
	}
}
