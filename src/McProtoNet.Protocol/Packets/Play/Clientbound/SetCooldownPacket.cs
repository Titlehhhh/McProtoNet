using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetCooldown", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SetCooldownPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 767),
        new(768, MinecraftVersion.LatestProtocol),
    };

    public int CooldownTicks { get; set; }
    public int ItemID { get; set; }

    public V768_769Fields? V768_769 { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                writer.WriteVarInt(ItemID);
                writer.WriteVarInt(CooldownTicks);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_769 ?? throw new InvalidOperationException("SetCooldown V768_769 fields missing.");
                writer.WriteString(fields.CooldownGroup);
                writer.WriteVarInt(CooldownTicks);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetCooldown), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                ItemID = reader.ReadVarInt();
                CooldownTicks = reader.ReadVarInt();
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V768_769Fields();
                fields.CooldownGroup = reader.ReadString();
                CooldownTicks = reader.ReadVarInt();
                V768_769 = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetCooldown), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V768_769Fields
    {
        public string CooldownGroup { get; set; }
    }

}
