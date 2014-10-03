using System;
using System.IO;
using Ionic.Zlib;
using ProtoBuf;

namespace Mercraft.Maps.Osm.Formats.Pbf
{
    /// <summary>
    ///     Reads PBF files.
    /// </summary>
    internal class PbfReader
    {
        /// <summary>
        ///     The stream containing the PBF data.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        ///     Creates a new PBF reader.
        /// </summary>
        /// <param name="stream"></param>
        public PbfReader(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        ///     Closes this reader.
        /// </summary>
        public void Dispose()
        {
            _stream.Dispose();
        }

        /// <summary>
        ///     Moves to the next primitive block, returns null at the end.
        /// </summary>
        /// <returns></returns>
        public PrimitiveBlock MoveNext()
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
                        header = Serializer.Deserialize<BlockHeader>(tmp);
                    }
                    Blob blob;
                    using (var tmp = new LimitedStream(_stream, header.datasize))
                    {
                        blob = Serializer.Deserialize<Blob>(tmp);
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
                            Serializer.Deserialize<HeaderBlock>(sourceStream);
                            notFoundBut = true;
                        }

                        if (header.type == "OSMData")
                        {
                            block = Serializer.Deserialize<PrimitiveBlock>(sourceStream);
                        }
                    }
                }
            }
            return block;
        }

        // 4-byte number
        private static int IntLittleEndianToBigEndian(uint i)
        {
            return (int) (((i & 0xff) << 24) + ((i & 0xff00) << 8) + ((i & 0xff0000) >> 8) + ((i >> 24) & 0xff));
        }
    }

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
}