using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ProfilelessChat", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ProfilelessChatPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(761, 764),
        new(765, 766),
        new(767, MinecraftVersion.LatestProtocol),
    };

    public V761_764Fields? V761_764 { get; set; }
    public V765_766Fields? V765_766 { get; set; }
    public V767_LastFields? V767_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 761 and <= 764:
            {
                var fields = V761_764 ?? throw new InvalidOperationException("ProfilelessChat V761_764 fields missing.");
                writer.WriteString(fields.Message);
                writer.WriteVarInt(fields.Type);
                writer.WriteString(fields.Name);
                if (fields.Target is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Target);
                }
                return;
            }
            case >= 765 and <= 766:
            {
                var fields = V765_766 ?? throw new InvalidOperationException("ProfilelessChat V765_766 fields missing.");
                writer.WriteAnonymousNbtTag(fields.Message, protocolVersion);
                writer.WriteVarInt(fields.Type);
                writer.WriteAnonymousNbtTag(fields.Name, protocolVersion);
                if (fields.Target is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(fields.Target, protocolVersion);
                }
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V767_Last ?? throw new InvalidOperationException("ProfilelessChat V767_Last fields missing.");
                writer.WriteAnonymousNbtTag(fields.Message, protocolVersion);
                writer.WriteChatTypesHolder(fields.Type, protocolVersion);
                writer.WriteAnonymousNbtTag(fields.Name, protocolVersion);
                if (fields.Target is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(fields.Target, protocolVersion);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ProfilelessChat), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 761 and <= 764:
                V761_764 = new V761_764Fields
                {
                    Message = reader.ReadString(),
                    Type = reader.ReadVarInt(),
                    Name = reader.ReadString(),
                    Target = reader.ReadOptional(ReadDelegates.String)
                };
                return;
            case >= 765 and <= 766:
                V765_766 = new V765_766Fields
                {
                    Message = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("ProfilelessChat message missing."),
                    Type = reader.ReadVarInt(),
                    Name = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("ProfilelessChat name missing."),
                    Target = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion))
                };
                return;
            case >= 767 and <= MinecraftVersion.LatestProtocol:
                V767_Last = new V767_LastFields
                {
                    Message = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("ProfilelessChat message missing."),
                    Type = reader.ReadChatTypesHolder(protocolVersion),
                    Name = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("ProfilelessChat name missing."),
                    Target = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion))
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ProfilelessChat), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V761_764Fields
    {
        public string Message { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string? Target { get; set; }
    }

    public struct V765_766Fields
    {
        public NbtTag Message { get; set; }
        public int Type { get; set; }
        public NbtTag Name { get; set; }
        public NbtTag? Target { get; set; }
    }

    public struct V767_LastFields
    {
        public NbtTag Message { get; set; }
        public ChatTypesHolder Type { get; set; }
        public NbtTag Name { get; set; }
        public NbtTag? Target { get; set; }
    }
}
