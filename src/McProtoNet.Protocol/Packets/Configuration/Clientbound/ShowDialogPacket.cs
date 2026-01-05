using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ShowDialog", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract class ShowDialogPacket : IServerPacket
{
    public NbtTag Dialog { get; set; } = null!;

    public static ShowDialogPacket Create() => new Impl();

    internal abstract void ReadPacket(ref MinecraftPrimitiveReader reader, int protocolVersion);
    internal abstract void WritePacket(ref MinecraftPrimitiveWriter writer, int protocolVersion);

    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => WritePacket(ref writer, protocolVersion);

    public void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => ReadPacket(ref reader, protocolVersion);

    private sealed class Impl : ShowDialogPacket
    {
        internal override void ReadPacket(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            switch (protocolVersion)
            {
                case >= 771 and <= 772:
                    Dialog = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("ShowDialog.dialog missing.");
                    break;
                default:
                    throw new ProtocolNotSupportException(nameof(ShowDialogPacket), protocolVersion);
            }
        }

        internal override void WritePacket(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            switch (protocolVersion)
            {
                case >= 771 and <= 772:
                    writer.WriteAnonymousNbtTag(Dialog, protocolVersion);
                    break;
                default:
                    throw new ProtocolNotSupportException(nameof(ShowDialogPacket), protocolVersion);
            }
        }
    }
}
