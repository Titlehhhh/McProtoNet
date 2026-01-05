using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ClearDialog", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract class ClearDialogPacket : IServerPacket
{
    public static ClearDialogPacket Create() => new Impl();

    internal abstract void ReadPacket(ref MinecraftPrimitiveReader reader, int protocolVersion);
    internal abstract void WritePacket(ref MinecraftPrimitiveWriter writer, int protocolVersion);

    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => WritePacket(ref writer, protocolVersion);

    public void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => ReadPacket(ref reader, protocolVersion);

    private sealed class Impl : ClearDialogPacket
    {
        internal override void ReadPacket(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            switch (protocolVersion)
            {
                case >= 771 and <= 772:
                    break;
                default:
                    throw new ProtocolNotSupportException(nameof(ClearDialogPacket), protocolVersion);
            }
        }

        internal override void WritePacket(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            switch (protocolVersion)
            {
                case >= 771 and <= 772:
                    break;
                default:
                    throw new ProtocolNotSupportException(nameof(ClearDialogPacket), protocolVersion);
            }
        }
    }
}
