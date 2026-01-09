using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("BossBar", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class BossBarPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public Guid EntityUUID { get; set; }
    public int Action { get; set; }

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("BossBar VFirst_764 fields missing.");
                writer.WriteUUID(EntityUUID);
                writer.WriteVarInt(Action);
                switch (Action)
                {
                    case 0:
                        writer.WriteString(fields.Title ?? throw new InvalidOperationException("BossBar title missing."));
                        writer.WriteFloat(fields.Health ?? throw new InvalidOperationException("BossBar health missing."));
                        writer.WriteVarInt(fields.Color ?? throw new InvalidOperationException("BossBar color missing."));
                        writer.WriteVarInt(fields.Dividers ?? throw new InvalidOperationException("BossBar dividers missing."));
                        writer.WriteUnsignedByte(fields.Flags ?? throw new InvalidOperationException("BossBar flags missing."));
                        break;
                    case 2:
                        writer.WriteFloat(fields.Health ?? throw new InvalidOperationException("BossBar health missing."));
                        break;
                    case 3:
                        writer.WriteString(fields.Title ?? throw new InvalidOperationException("BossBar title missing."));
                        break;
                    case 4:
                        writer.WriteVarInt(fields.Color ?? throw new InvalidOperationException("BossBar color missing."));
                        writer.WriteVarInt(fields.Dividers ?? throw new InvalidOperationException("BossBar dividers missing."));
                        break;
                    case 5:
                        writer.WriteUnsignedByte(fields.Flags ?? throw new InvalidOperationException("BossBar flags missing."));
                        break;
                }
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("BossBar V765_Last fields missing.");
                writer.WriteUUID(EntityUUID);
                writer.WriteVarInt(Action);
                switch (Action)
                {
                    case 0:
                        writer.WriteAnonymousNbtTag(fields.Title ?? throw new InvalidOperationException("BossBar title missing."), protocolVersion);
                        writer.WriteFloat(fields.Health ?? throw new InvalidOperationException("BossBar health missing."));
                        writer.WriteVarInt(fields.Color ?? throw new InvalidOperationException("BossBar color missing."));
                        writer.WriteVarInt(fields.Dividers ?? throw new InvalidOperationException("BossBar dividers missing."));
                        writer.WriteUnsignedByte(fields.Flags ?? throw new InvalidOperationException("BossBar flags missing."));
                        break;
                    case 2:
                        writer.WriteFloat(fields.Health ?? throw new InvalidOperationException("BossBar health missing."));
                        break;
                    case 3:
                        writer.WriteAnonymousNbtTag(fields.Title ?? throw new InvalidOperationException("BossBar title missing."), protocolVersion);
                        break;
                    case 4:
                        writer.WriteVarInt(fields.Color ?? throw new InvalidOperationException("BossBar color missing."));
                        writer.WriteVarInt(fields.Dividers ?? throw new InvalidOperationException("BossBar dividers missing."));
                        break;
                    case 5:
                        writer.WriteUnsignedByte(fields.Flags ?? throw new InvalidOperationException("BossBar flags missing."));
                        break;
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.BossBar), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = new VFirst_764Fields();
                EntityUUID = reader.ReadUUID();
                Action = reader.ReadVarInt();
                switch (Action)
                {
                    case 0:
                        fields.Title = reader.ReadString();
                        fields.Health = reader.ReadFloat();
                        fields.Color = reader.ReadVarInt();
                        fields.Dividers = reader.ReadVarInt();
                        fields.Flags = reader.ReadUnsignedByte();
                        break;
                    case 2:
                        fields.Health = reader.ReadFloat();
                        break;
                    case 3:
                        fields.Title = reader.ReadString();
                        break;
                    case 4:
                        fields.Color = reader.ReadVarInt();
                        fields.Dividers = reader.ReadVarInt();
                        break;
                    case 5:
                        fields.Flags = reader.ReadUnsignedByte();
                        break;
                }
                VFirst_764 = fields;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V765_LastFields();
                EntityUUID = reader.ReadUUID();
                Action = reader.ReadVarInt();
                switch (Action)
                {
                    case 0:
                        fields.Title = reader.ReadAnonymousNbtTag(protocolVersion)
                            ?? throw new InvalidOperationException("BossBar title missing.");
                        fields.Health = reader.ReadFloat();
                        fields.Color = reader.ReadVarInt();
                        fields.Dividers = reader.ReadVarInt();
                        fields.Flags = reader.ReadUnsignedByte();
                        break;
                    case 2:
                        fields.Health = reader.ReadFloat();
                        break;
                    case 3:
                        fields.Title = reader.ReadAnonymousNbtTag(protocolVersion)
                            ?? throw new InvalidOperationException("BossBar title missing.");
                        break;
                    case 4:
                        fields.Color = reader.ReadVarInt();
                        fields.Dividers = reader.ReadVarInt();
                        break;
                    case 5:
                        fields.Flags = reader.ReadUnsignedByte();
                        break;
                }
                V765_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.BossBar), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_764Fields
    {
        public string? Title { get; set; }
        public float? Health { get; set; }
        public int? Color { get; set; }
        public int? Dividers { get; set; }
        public byte? Flags { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag? Title { get; set; }
        public float? Health { get; set; }
        public int? Color { get; set; }
        public int? Dividers { get; set; }
        public byte? Flags { get; set; }
    }
}
