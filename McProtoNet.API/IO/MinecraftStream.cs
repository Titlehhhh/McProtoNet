namespace McProtoNet.API.IO
{
    /// <summary>
    /// Поток обеспечивающий работу с примитивными данными протокола Майкрафт    /// 
    /// </summary>
    public sealed partial class MinecraftStream : Stream, IMinecraftPrimitiveReader, IMinecraftPrimitiveWriter
    {
        public Stream BaseStream { get; set; }

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public CancellationTokenSource Cancellation { get; set; }

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
        public MinecraftStream() : this(new MemoryStream())
        {

        }
        public MinecraftStream(Stream stream)
        {
            BaseStream = stream;

        }
        public MinecraftStream(byte[] data) : this(new MemoryStream(data))
        {

        }
        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }
        /// <summary>
        /// asd
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }
        /// <summary>
        /// asd
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public override int Read(Span<byte> buffer)
        {
            return BaseStream.Read(buffer);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            BaseStream.Write(buffer);
        }


    }
    public static class GuidExtensions
    {
        public static Guid ToLittleEndian(this Guid javaGuid)
        {
            byte[] net = new byte[16];
            byte[] java = javaGuid.ToByteArray();
            for (int i = 8; i < 16; i++)
            {
                net[i] = java[i];
            }
            net[3] = java[0];
            net[2] = java[1];
            net[1] = java[2];
            net[0] = java[3];
            net[5] = java[4];
            net[4] = java[5];
            net[6] = java[7];
            net[7] = java[6];
            return new Guid(net);
        }

        /// <summary>
        /// Converts little-endian .NET guids to big-endian Java guids:
        /// </summary>
        public static Guid ToBigEndian(this Guid netGuid)
        {
            byte[] java = new byte[16];
            byte[] net = netGuid.ToByteArray();
            for (int i = 8; i < 16; i++)
            {
                java[i] = net[i];
            }
            java[0] = net[3];
            java[1] = net[2];
            java[2] = net[1];
            java[3] = net[0];
            java[4] = net[5];
            java[5] = net[4];
            java[6] = net[7];
            java[7] = net[6];
            return new Guid(java);
        }

        /// <summary>
        /// Converts little-endian .NET guids to big-endian Java guids:
        /// </summary>
        public static byte[] ToBigEndianBytes(this Guid netGuid)
        {
            byte[] java = new byte[16];
            byte[] net = netGuid.ToByteArray();
            for (int i = 8; i < 16; i++)
            {
                java[i] = net[i];
            }
            java[0] = net[3];
            java[1] = net[2];
            java[2] = net[1];
            java[3] = net[0];
            java[4] = net[5];
            java[5] = net[4];
            java[6] = net[7];
            java[7] = net[6];
            return java;
        }
    }
}
