using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zlib;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Mercraft.Maps.Osm.Formats.Pbf
{
    /// <summary>
    ///     Reads PBF files.
    /// </summary>
    internal class PbfReader : IEnumerator<PrimitiveBlock>
    {
        /// <summary>
        ///     The stream containing the PBF data.
        /// </summary>
        private Stream _stream;

        private readonly RuntimeTypeModel _runtimeTypeModel;

        // Types of the objects to be deserialized.
        private readonly Type _blockHeaderType = typeof(BlockHeader);
        private readonly Type _blobType = typeof(Blob);
        private readonly Type _primitiveBlockType = typeof(PrimitiveBlock);
        private readonly Type _headerBlockType = typeof(HeaderBlock);

        private readonly List<PrimitiveBlock> _blocks = new List<PrimitiveBlock>(16);

        private int _currentIndex = 0;

        /// <summary>
        ///     Creates a new PBF reader.
        /// </summary>
        public PbfReader()
        {
            _runtimeTypeModel = TypeModel.Create();
            _runtimeTypeModel.Add(_blockHeaderType, true);
            _runtimeTypeModel.Add(_blobType, true);
            _runtimeTypeModel.Add(_primitiveBlockType, true);
            _runtimeTypeModel.Add(_headerBlockType, true);
        }

        /// <summary>
        ///     Sets stream. This have side effect: cached data will be erased.
        /// </summary>
        public void SetStream(Stream stream)
        {
            _stream = stream;
            _blocks.Clear();
            Reset();
        }

        #region IEnumerable implementation

        /// <summary>
        ///     Processes next element in sequence. Supports caching of deserialized data.
        /// </summary>
        public bool MoveNext()
        {
            if (!_blocks.Any())
            {
                PrimitiveBlock block = null;
                while ((block = ProcessBlock()) != null)
                    _blocks.Add(block);
            }

            return _currentIndex++ != _blocks.Count;
        }

        public void Reset()
        {
            if (_stream.CanSeek)
                _stream.Seek(0, SeekOrigin.Begin);
            _currentIndex = 0;
        }

        public PrimitiveBlock Current
        {
            get
            {
                return _blocks[_currentIndex - 1];
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion

        /// <summary>
        ///     Read next block. Use this API if you don't want to cache content.
        /// </summary>
        public PrimitiveBlock ProcessBlock()
        {
            PrimitiveBlock block = null;
            bool notFoundBut = true;
            while (notFoundBut)
            {
                // continue if there is still data but not a primitiveblock.
                notFoundBut = false; // not found.
                int length;
                if (Serializer.TryReadLengthPrefix(_stream, PrefixStyle.Fixed32, out length))
                {
                    // TODO: remove some of the v1 specific code.
                    // TODO: this means also to use the built-in capped streams.

                    // code borrowed from: http://stackoverflow.com/questions/4663298/protobuf-net-deserialize-open-street-maps

                    // I'm just being lazy and re-using something "close enough" here
                    // note that v2 has a big-endian option, but Fixed32 assumes little-endian - we
                    // actually need the other way around (network byte order):
                    length = IntLittleEndianToBigEndian((uint) length);

                    BlockHeader header;
                    // again, v2 has capped-streams built in, but I'm deliberately
                    // limiting myself to v1 features
                    using (var tmp = new LimitedStream(_stream, length))
                    {
                        header = _runtimeTypeModel.Deserialize(tmp, null, _blockHeaderType) as BlockHeader;
                    }
                    Blob blob;
                    using (var tmp = new LimitedStream(_stream, header.datasize))
                    {
                        blob = _runtimeTypeModel.Deserialize(tmp, null, _blobType) as Blob;
                    }

                    // construct the source stream, compressed or not.
                    Stream sourceStream;
                    if (blob.zlib_data == null)
                    {
                        // use a regular uncompressed stream.
                        sourceStream = new MemoryStream(blob.raw);
                    }
                    else
                    {
                        // construct a compressed stream.
                        var ms = new MemoryStream(blob.zlib_data);
                        sourceStream = new ZLibStreamWrapper(ms);
                    }

                    // use the stream to read the block.
                    using (sourceStream)
                    {
                        if (header.type == "OSMHeader")
                        {
                            _runtimeTypeModel.Deserialize(sourceStream, null, _headerBlockType);
                            notFoundBut = true;
                        }

                        if (header.type == "OSMData")
                        {
                            block = _runtimeTypeModel.Deserialize(sourceStream, block, _primitiveBlockType) as PrimitiveBlock;
                        }
                    }
                }
            }
            return block;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _stream.Dispose();
            _stream = null;
            _blocks.Clear();
        }

        // 4-byte number
        private static int IntLittleEndianToBigEndian(uint i)
        {
            return (int) (((i & 0xff) << 24) + ((i & 0xff00) << 8) + ((i & 0xff0000) >> 8) + ((i >> 24) & 0xff));
        }
    }

    #region Stream classes

    internal abstract class InputStream : Stream
    {
        private long _pos;

        protected abstract int ReadNextBlock(byte[] buffer, int offset, int count);

        public override sealed int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead, totalRead = 0;
            while (count > 0 && (bytesRead = ReadNextBlock(buffer, offset, count)) > 0)
            {
                count -= bytesRead;
                offset += bytesRead;
                totalRead += bytesRead;
                _pos += bytesRead;
            }
            return totalRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override long Position
        {
            get { return _pos; }
            set { if (_pos != value) throw new NotImplementedException(); }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }
    }

    internal class ZLibStreamWrapper : InputStream
    {
        private readonly ZlibStream _reader;

        public ZLibStreamWrapper(Stream stream)
        {
            _reader = new ZlibStream(stream, CompressionMode.Decompress);
        }

        protected override int ReadNextBlock(byte[] buffer, int offset, int count)
        {
            return _reader.Read(buffer, offset, count);
        }
    }

    // deliberately doesn't dispose the base-stream    
    internal class LimitedStream : InputStream
    {
        private readonly Stream _stream;
        private long _remaining;

        public LimitedStream(Stream stream, long length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length");
            if (stream == null) throw new ArgumentNullException("stream");
            if (!stream.CanRead) throw new ArgumentException("stream");
            _stream = stream;
            _remaining = length;
        }

        protected override int ReadNextBlock(byte[] buffer, int offset, int count)
        {
            if (count > _remaining) count = (int) _remaining;
            int bytesRead = _stream.Read(buffer, offset, count);
            if (bytesRead > 0) _remaining -= bytesRead;
            return bytesRead;
        }
    }
    
    #endregion
}