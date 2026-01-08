﻿using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ServerData", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ServerDataPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(759, 759),
        new(760, 760),
        new(761, 761),
        new(762, 764),
        new(765, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public V759Fields? V759 { get; set; }
    public V760Fields? V760 { get; set; }
    public V761Fields? V761 { get; set; }
    public V762_764Fields? V762_764 { get; set; }
    public V765Fields? V765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 759:
            {
                var fields = V759 ?? throw new InvalidOperationException("ServerData V759 missing.");
                if (fields.Motd is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Motd);
                }
                if (fields.Icon is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Icon);
                }
                writer.WriteBoolean(fields.PreviewsChat);
                return;
            }
            case 760:
            {
                var fields = V760 ?? throw new InvalidOperationException("ServerData V760 missing.");
                if (fields.Motd is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Motd);
                }
                if (fields.Icon is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Icon);
                }
                writer.WriteBoolean(fields.PreviewsChat);
                writer.WriteBoolean(fields.EnforcesSecureChat);
                return;
            }
            case 761:
            {
                var fields = V761 ?? throw new InvalidOperationException("ServerData V761 missing.");
                if (fields.Motd is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Motd);
                }
                if (fields.Icon is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Icon);
                }
                writer.WriteBoolean(fields.EnforcesSecureChat);
                return;
            }
            case >= 762 and <= 764:
            {
                var fields = V762_764 ?? throw new InvalidOperationException("ServerData V762_764 missing.");
                writer.WriteString(fields.Motd);
                if (fields.IconBytes is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVarInt(fields.IconBytes.Length);
                    writer.WriteBuffer(fields.IconBytes);
                }
                writer.WriteBoolean(fields.EnforcesSecureChat);
                return;
            }
            case 765:
            {
                var fields = V765 ?? throw new InvalidOperationException("ServerData V765 missing.");
                writer.WriteAnonymousNbtTag(fields.Motd, protocolVersion);
                if (fields.IconBytes is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVarInt(fields.IconBytes.Length);
                    writer.WriteBuffer(fields.IconBytes);
                }
                writer.WriteBoolean(fields.EnforcesSecureChat);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("ServerData V766_Last missing.");
                writer.WriteAnonymousNbtTag(fields.Motd, protocolVersion);
                if (fields.IconBytes is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVarInt(fields.IconBytes.Length);
                    writer.WriteBuffer(fields.IconBytes);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ServerData), protocolVersion, SupportedVersionsStatic);
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
                    Motd = reader.ReadOptional(ReadDelegates.String),
                    Icon = reader.ReadOptional(ReadDelegates.String),
                    PreviewsChat = reader.ReadBoolean()
                };
                V759 = fields;
                return;
            }
            case 760:
            {
                var fields = new V760Fields
                {
                    Motd = reader.ReadOptional(ReadDelegates.String),
                    Icon = reader.ReadOptional(ReadDelegates.String),
                    PreviewsChat = reader.ReadBoolean(),
                    EnforcesSecureChat = reader.ReadBoolean()
                };
                V760 = fields;
                return;
            }
            case 761:
            {
                var fields = new V761Fields
                {
                    Motd = reader.ReadOptional(ReadDelegates.String),
                    Icon = reader.ReadOptional(ReadDelegates.String),
                    EnforcesSecureChat = reader.ReadBoolean()
                };
                V761 = fields;
                return;
            }
            case >= 762 and <= 764:
            {
                var fields = new V762_764Fields
                {
                    Motd = reader.ReadString(),
                    IconBytes = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadBuffer(r.ReadVarInt())),
                    EnforcesSecureChat = reader.ReadBoolean()
                };
                V762_764 = fields;
                return;
            }
            case 765:
            {
                var fields = new V765Fields
                {
                    Motd = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("ServerData Motd missing."),
                    IconBytes = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadBuffer(r.ReadVarInt())),
                    EnforcesSecureChat = reader.ReadBoolean()
                };
                V765 = fields;
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V766_LastFields
                {
                    Motd = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("ServerData Motd missing."),
                    IconBytes = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadBuffer(r.ReadVarInt()))
                };
                V766_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ServerData), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759Fields
    {
        public string? Motd { get; set; }
        public string? Icon { get; set; }
        public bool PreviewsChat { get; set; }
    }

    public struct V760Fields
    {
        public string? Motd { get; set; }
        public string? Icon { get; set; }
        public bool PreviewsChat { get; set; }
        public bool EnforcesSecureChat { get; set; }
    }

    public struct V761Fields
    {
        public string? Motd { get; set; }
        public string? Icon { get; set; }
        public bool EnforcesSecureChat { get; set; }
    }

    public struct V762_764Fields
    {
        public string Motd { get; set; }
        public byte[]? IconBytes { get; set; }
        public bool EnforcesSecureChat { get; set; }
    }

    public struct V765Fields
    {
        public NbtTag Motd { get; set; }
        public byte[]? IconBytes { get; set; }
        public bool EnforcesSecureChat { get; set; }
    }

    public struct V766_LastFields
    {
        public NbtTag Motd { get; set; }
        public byte[]? IconBytes { get; set; }
    }
}
