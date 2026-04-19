using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public struct VarInt;

public struct VarLong;

public static partial class ProtocolSerializationExtensions
{
    extension(ref MinecraftPrimitiveReader reader)
    {
        public NbtTag? ReadNbtTag(int protocolVersion)
        {
            return reader.ReadNbtTag(protocolVersion < 764);
        }

        public NbtTag? ReadOptionalNbtTag(int protocolVersion)
        {
            return reader.ReadOptionalNbtTag(protocolVersion < 764);
        }

        public NbtTag? ReadAnonymousNbtTag(int protocolVersion)
        {
            return reader.ReadNbtTag(readRootTag: false);
        }

        public NbtTag? ReadAnonOptionalNbtTag(int protocolVersion)
        {
            return reader.ReadOptionalNbtTag(readRootTag: false);
        }
    }

    extension(MinecraftPrimitiveWriter writer)
    {
        public void WriteNbtTag(NbtTag value, int protocolVersion)
        {
            ArgumentNullException.ThrowIfNull(writer);
            writer.WriteNbt(value);
        }

        public void WriteOptionalNbtTag(NbtTag? value, int protocolVersion)
        {
            ArgumentNullException.ThrowIfNull(writer);
            writer.WriteOptionalNbt(value);
        }

        public void WriteAnonymousNbtTag(NbtTag value, int protocolVersion)
        {
            ArgumentNullException.ThrowIfNull(writer);
            writer.WriteNbt(value);
        }

        public void WriteAnonOptionalNbtTag(NbtTag? value, int protocolVersion)
        {
            ArgumentNullException.ThrowIfNull(writer);
            writer.WriteOptionalNbt(value);
        }
    }
}