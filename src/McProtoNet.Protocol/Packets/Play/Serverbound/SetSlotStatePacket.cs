using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SetSlotState", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class SetSlotStatePacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(765, MinecraftVersion.LatestProtocol)
    };

    public int SlotId { get; set; }
    public int WindowId { get; set; }
    public bool State { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 765 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(SlotId);
                writer.WriteVarInt(WindowId);
                writer.WriteBoolean(State);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.SetSlotState), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 765 and <= MinecraftVersion.LatestProtocol:
                SlotId = reader.ReadVarInt();
                WindowId = reader.ReadVarInt();
                State = reader.ReadBoolean();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.SetSlotState), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
