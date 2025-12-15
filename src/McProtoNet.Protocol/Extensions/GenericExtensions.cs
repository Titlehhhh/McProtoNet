using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static class GenericExtensions
{
    extension(ref MinecraftPrimitiveWriter writer)
    {
        public void WriteArray<T>(ReadOnlySpan<T> arr)
        {
            //TODO
        }

        public void WriteBuffer(ReadOnlySpan<byte> buff, int length)
        {
        }

        public void WriteBuffer<TLen>(ReadOnlySpan<byte> buff)
        {
            if (typeof(TLen) == typeof(VarInt))
            {
                writer.WriteVarInt(buff.Length);
                writer.WriteBuffer(buff);
            }
        }


        public void WriteType<T>(T val)
        {
        }
    }

    extension(ref MinecraftPrimitiveReader reader)
    {
        public Position ReadPosition(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Position>(protocolVersion);
            var locEncoded = reader.ReadSignedLong();
            var x = (int)(locEncoded >> 38);
            var z = (int)((locEncoded >> 12) & 0x3FFFFFF);
            var y = (int)(locEncoded & 0xFFF);
            return new Position(x, y, z);
        }
    }
}