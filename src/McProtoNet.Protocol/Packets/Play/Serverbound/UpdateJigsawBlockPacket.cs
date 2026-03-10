using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UpdateJigsawBlock", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class UpdateJigsawBlockPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, MinecraftVersion.LatestProtocol)
    };

    public Position Location { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Pool { get; set; } = string.Empty;
    public string FinalState { get; set; } = string.Empty;
    public string JointType { get; set; } = string.Empty;

    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
                writer.WritePosition(Location, protocolVersion);
                writer.WriteString(Name);
                writer.WriteString(Target);
                writer.WriteString(Pool);
                writer.WriteString(FinalState);
                writer.WriteString(JointType);
                return;
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("UpdateJigsawBlock V765_Last fields missing.");
                writer.WritePosition(Location, protocolVersion);
                writer.WriteString(Name);
                writer.WriteString(Target);
                writer.WriteString(Pool);
                writer.WriteString(FinalState);
                writer.WriteString(JointType);
                writer.WriteVarInt(fields.SelectionPriority);
                writer.WriteVarInt(fields.PlacementPriority);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UpdateJigsawBlock), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
                Location = reader.ReadPosition(protocolVersion);
                Name = reader.ReadString();
                Target = reader.ReadString();
                Pool = reader.ReadString();
                FinalState = reader.ReadString();
                JointType = reader.ReadString();
                V765_Last = null;
                return;
            case >= 765 and <= MinecraftVersion.LatestProtocol:
                Location = reader.ReadPosition(protocolVersion);
                Name = reader.ReadString();
                Target = reader.ReadString();
                Pool = reader.ReadString();
                FinalState = reader.ReadString();
                JointType = reader.ReadString();
                V765_Last = new V765_LastFields
                {
                    SelectionPriority = reader.ReadVarInt(),
                    PlacementPriority = reader.ReadVarInt()
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UpdateJigsawBlock), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V765_LastFields
    {
        public int SelectionPriority { get; set; }
        public int PlacementPriority { get; set; }
    }
}
