﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SharpCompress.Readers;

namespace SharpCompress.Common
{
    public class EntryStream : Stream
    {
        private readonly IReader _reader;
        private readonly Stream _stream;
        private bool _completed;
        private bool _isDisposed;

        internal EntryStream(IReader reader, Stream stream)
        {
            _reader = reader;
            _stream = stream;
        }

        /// <summary>
        /// When reading a stream from OpenEntryStream, the stream must be completed so use this to finish reading the entire entry.
        /// </summary>
        public async ValueTask SkipEntryAsync(CancellationToken cancellationToken = default)
        {
            await this.SkipAsync(cancellationToken);
            _completed = true;
        }

        public override async ValueTask DisposeAsync()
        {
            if (!(_completed || _reader.Cancelled))
            {
                await SkipEntryAsync();
            }
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            await _stream.DisposeAsync();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override void Flush()
        {
        }

        public override long Length => _stream.Length;

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int read = await _stream.ReadAsync(buffer, cancellationToken);
            if (read <= 0)
            {
                _completed = true;
            }
            return read;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override int ReadByte()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}