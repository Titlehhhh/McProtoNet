﻿using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SystemChat", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SystemChatPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(759, 759),
        new(760, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public V759Fields? V759 { get; set; }
    public V760_764Fields? V760_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 759:
            {
                var fields = V759 ?? throw new InvalidOperationException("SystemChat V759 missing.");
                writer.WriteString(fields.Content);
                writer.WriteVarInt(fields.Type);
                return;
            }
            case >= 760 and <= 764:
            {
                var fields = V760_764 ?? throw new InvalidOperationException("SystemChat V760_764 missing.");
                writer.WriteString(fields.Content);
                writer.WriteBoolean(fields.IsActionBar);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("SystemChat V765_Last missing.");
                writer.WriteAnonymousNbtTag(fields.Content, protocolVersion);
                writer.WriteBoolean(fields.IsActionBar);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SystemChat), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 759:
            {
                var fields = new V759Fields
                {
                    Content = reader.ReadString(),
                    Type = reader.ReadVarInt()
                };
                V759 = fields;
                return;
            }
            case >= 760 and <= 764:
            {
                var fields = new V760_764Fields
                {
                    Content = reader.ReadString(),
                    IsActionBar = reader.ReadBoolean()
                };
                V760_764 = fields;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V765_LastFields
                {
                    Content = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("SystemChat Content missing."),
                    IsActionBar = reader.ReadBoolean()
                };
                V765_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SystemChat), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759Fields
    {
        public string Content { get; set; }
        public int Type { get; set; }
    }

    public struct V760_764Fields
    {
        public string Content { get; set; }
        public bool IsActionBar { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag Content { get; set; }
        public bool IsActionBar { get; set; }
    }
}
