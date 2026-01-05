using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("CustomClickAction", PacketState.Configuration, PacketDirection.Serverbound)]
public abstract class CustomClickActionPacket : IClientPacket
{
    public string Id { get; set; } = string.Empty;
    public NbtTag? Nbt { get; set; }

    public static CustomClickActionPacket Create() => new Impl();

    internal abstract void ReadPacket(ref MinecraftPrimitiveReader reader, int protocolVersion);
    internal abstract void WritePacket(ref MinecraftPrimitiveWriter writer, int protocolVersion);

    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => WritePacket(ref writer, protocolVersion);

    public void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => ReadPacket(ref reader, protocolVersion);

    private sealed class Impl : CustomClickActionPacket
    {
        internal override void ReadPacket(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            switch (protocolVersion)
            {
                case >= 771 and <= 772:
                    Id = reader.ReadString();
                    Nbt = reader.ReadAnonOptionalNbtTag(protocolVersion);
                    break;
                default:
                    throw new ProtocolNotSupportException(nameof(CustomClickActionPacket), protocolVersion);
            }
        }

        internal override void WritePacket(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            switch (protocolVersion)
            {
                case >= 771 and <= 772:
                    writer.WriteString(Id);
                    writer.WriteAnonOptionalNbtTag(Nbt, protocolVersion);
                    break;
                default:
                    throw new ProtocolNotSupportException(nameof(CustomClickActionPacket), protocolVersion);
            }
        }
    }
}
