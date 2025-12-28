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

        public void WriteVec2f(Vec2f vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec2f>(protocolVersion);
            writer.WriteFloat(vec.x);
            writer.WriteFloat(vec.y);
        }

        public void WriteVec3f(Vec3f vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3f>(protocolVersion);
            writer.WriteFloat(vec.x);
            writer.WriteFloat(vec.y);
            writer.WriteFloat(vec.z);
        }

        public void WriteVec3f64(Vec3f64 vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3f64>(protocolVersion);
            writer.WriteDouble(vec.x);
            writer.WriteDouble(vec.y);
            writer.WriteDouble(vec.z);
        }

        public void WriteVec3i(Vec3i vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3i>(protocolVersion);
            writer.WriteVarInt(vec.x);
            writer.WriteVarInt(vec.y);
            writer.WriteVarInt(vec.z);
        }

        public void WriteVec4f(Vec4f vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec4f>(protocolVersion);
            writer.WriteFloat(vec.x);
            writer.WriteFloat(vec.y);
            writer.WriteFloat(vec.z);
            writer.WriteFloat(vec.w);
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

        public Vec2f ReadVec2f(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec2f>(protocolVersion);
            var x = reader.ReadFloat();
            var y = reader.ReadFloat();
            return new Vec2f(x, y);
        }

        public Vec3f ReadVec3f(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3f>(protocolVersion);
            var x = reader.ReadFloat();
            var y = reader.ReadFloat();
            var z = reader.ReadFloat();
            return new Vec3f(x, y, z);
        }

        public Vec3f64 ReadVec3f64(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3f64>(protocolVersion);
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            return new Vec3f64(x, y, z);
        }

        public Vec3i ReadVec3i(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3i>(protocolVersion);
            var x = reader.ReadVarInt();
            var y = reader.ReadVarInt();
            var z = reader.ReadVarInt();
            return new Vec3i(x, y, z);
        }

        public Vec4f ReadVec4f(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec4f>(protocolVersion);
            var x = reader.ReadFloat();
            var y = reader.ReadFloat();
            var z = reader.ReadFloat();
            var w = reader.ReadFloat();
            return new Vec4f(x, y, z, w);
        }
    }
}
