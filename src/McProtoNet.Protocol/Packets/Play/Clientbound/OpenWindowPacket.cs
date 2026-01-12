using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("OpenWindow", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class OpenWindowPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public int WindowId { get; set; }
    public int InventoryType { get; set; }

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("OpenWindow VFirst_764 fields missing.");
                writer.WriteVarInt(WindowId);
                writer.WriteVarInt(InventoryType);
                writer.WriteString(fields.WindowTitle);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("OpenWindow V765_Last fields missing.");
                writer.WriteVarInt(WindowId);
                writer.WriteVarInt(InventoryType);
                writer.WriteAnonymousNbtTag(fields.WindowTitle, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.OpenWindow), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
                WindowId = reader.ReadVarInt();
                InventoryType = reader.ReadVarInt();
                VFirst_764 = new VFirst_764Fields
                {
                    WindowTitle = reader.ReadString()
                };
                return;
            case >= 765 and <= MinecraftVersion.LatestProtocol:
                WindowId = reader.ReadVarInt();
                InventoryType = reader.ReadVarInt();
                V765_Last = new V765_LastFields
                {
                    WindowTitle = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("OpenWindow title missing.")
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.OpenWindow), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_764Fields
    {
        public string WindowTitle { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag WindowTitle { get; set; }
    }
}
