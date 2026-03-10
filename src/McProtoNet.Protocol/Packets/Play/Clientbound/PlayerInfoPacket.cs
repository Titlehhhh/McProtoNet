using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("PlayerInfo", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class PlayerInfoPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 760),
        new(761, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public int Action { get; set; }

    public VFirst_760Fields? VFirst_760 { get; set; }
    public V761_764Fields? V761_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 760:
            {
                var fields = VFirst_760 ?? throw new InvalidOperationException("PlayerInfo VFirst_760 fields missing.");
                writer.WriteVarInt(Action);
                writer.WriteVarInt(fields.Data.Length);
                for (int i = 0; i < fields.Data.Length; i++)
                {
                    WriteLegacyEntry(ref writer, fields.Data[i], Action, protocolVersion, protocolVersion >= 759);
                }
                return;
            }
            case >= 761 and <= 764:
            {
                var fields = V761_764 ?? throw new InvalidOperationException("PlayerInfo V761_764 fields missing.");
                byte flags = BuildFlags(fields.Flags);
                writer.WriteUnsignedByte(flags);
                writer.WriteVarInt(fields.Data.Length);
                for (int i = 0; i < fields.Data.Length; i++)
                {
                    WriteModernEntry(ref writer, fields.Data[i], fields.Flags, protocolVersion, useAnonymous: false);
                }
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("PlayerInfo V765_Last fields missing.");
                byte flags = BuildFlags(fields.Flags);
                writer.WriteUnsignedByte(flags);
                writer.WriteVarInt(fields.Data.Length);
                for (int i = 0; i < fields.Data.Length; i++)
                {
                    WriteModernEntry(ref writer, fields.Data[i], fields.Flags, protocolVersion, useAnonymous: true);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.PlayerInfo), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 760:
            {
                Action = reader.ReadVarInt();
                int count = reader.ReadVarInt();
                var data = new LegacyEntry[count];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = ReadLegacyEntry(ref reader, Action, protocolVersion, protocolVersion >= 759);
                }
                VFirst_760 = new VFirst_760Fields { Data = data };
                return;
            }
            case >= 761 and <= 764:
            {
                var flags = ReadFlags(reader.ReadUnsignedByte());
                int count = reader.ReadVarInt();
                var data = new ModernEntry[count];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = ReadModernEntry(ref reader, flags, protocolVersion, useAnonymous: false);
                }
                V761_764 = new V761_764Fields { Flags = flags, Data = data };
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var flags = ReadFlags(reader.ReadUnsignedByte());
                int count = reader.ReadVarInt();
                var data = new ModernEntry[count];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = ReadModernEntry(ref reader, flags, protocolVersion, useAnonymous: true);
                }
                V765_Last = new V765_LastFields { Flags = flags, Data = data };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.PlayerInfo), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static LegacyEntry ReadLegacyEntry(ref MinecraftPrimitiveReader reader, int action, int protocolVersion, bool hasCrypto)
    {
        var entry = new LegacyEntry { Uuid = reader.ReadUUID() };
        switch (action)
        {
            case 0:
            {
                var add = new LegacyAddPlayer
                {
                    Name = reader.ReadString()
                };
                int propCount = reader.ReadVarInt();
                add.Properties = new PlayerProperty[propCount];
                for (int i = 0; i < add.Properties.Length; i++)
                {
                    add.Properties[i] = new PlayerProperty
                    {
                        Name = reader.ReadString(),
                        Value = reader.ReadString(),
                        Signature = reader.ReadOptional(ReadDelegates.String)
                    };
                }
                add.GameMode = reader.ReadVarInt();
                add.Ping = reader.ReadVarInt();
                add.DisplayName = reader.ReadOptional(ReadDelegates.String);
                if (hasCrypto)
                {
                    add.Crypto = reader.ReadOptional((ref MinecraftPrimitiveReader r) =>
                    {
                        return new CryptoData
                        {
                            Timestamp = r.ReadSignedLong(),
                            PublicKey = r.ReadBuffer(LengthFormat.VarInt),
                            Signature = r.ReadBuffer(LengthFormat.VarInt)
                        };
                    });
                }
                entry.AddPlayer = add;
                break;
            }
            case 1:
                entry.GameMode = reader.ReadVarInt();
                break;
            case 2:
                entry.Ping = reader.ReadVarInt();
                break;
            case 3:
                entry.DisplayName = reader.ReadOptional(ReadDelegates.String);
                break;
        }
        return entry;
    }

    private static void WriteLegacyEntry(ref MinecraftPrimitiveWriter writer, LegacyEntry entry, int action, int protocolVersion, bool hasCrypto)
    {
        writer.WriteUUID(entry.Uuid);
        switch (action)
        {
            case 0:
            {
                var add = entry.AddPlayer ?? throw new InvalidOperationException("PlayerInfo add_player missing.");
                writer.WriteString(add.Name);
                writer.WriteVarInt(add.Properties.Length);
                for (int i = 0; i < add.Properties.Length; i++)
                {
                    writer.WriteString(add.Properties[i].Name);
                    writer.WriteString(add.Properties[i].Value);
                    if (add.Properties[i].Signature is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteString(add.Properties[i].Signature!);
                    }
                }
                writer.WriteVarInt(add.GameMode);
                writer.WriteVarInt(add.Ping);
                if (add.DisplayName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(add.DisplayName!);
                }
                if (hasCrypto)
                {
                    if (add.Crypto is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteSignedLong(add.Crypto.Value.Timestamp);
                        writer.WriteBuffer<VarInt>(add.Crypto.Value.PublicKey);
                        writer.WriteBuffer<VarInt>(add.Crypto.Value.Signature);
                    }
                }
                break;
            }
            case 1:
                writer.WriteVarInt(entry.GameMode ?? throw new InvalidOperationException("PlayerInfo gamemode missing."));
                break;
            case 2:
                writer.WriteVarInt(entry.Ping ?? throw new InvalidOperationException("PlayerInfo ping missing."));
                break;
            case 3:
                if (entry.DisplayName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(entry.DisplayName!);
                }
                break;
        }
    }

    private static ModernEntry ReadModernEntry(ref MinecraftPrimitiveReader reader, PlayerInfoFlags flags, int protocolVersion, bool useAnonymous)
    {
        var entry = new ModernEntry { Uuid = reader.ReadUUID() };
        if (flags.AddPlayer)
        {
            entry.Player = reader.ReadGameProfile(protocolVersion);
        }
        if (flags.InitializeChat)
        {
            entry.ChatSession = reader.ReadChatSession(protocolVersion);
        }
        if (flags.UpdateGameMode)
        {
            entry.GameMode = reader.ReadVarInt();
        }
        if (flags.UpdateListed)
        {
            entry.Listed = reader.ReadVarInt();
        }
        if (flags.UpdateLatency)
        {
            entry.Latency = reader.ReadVarInt();
        }
        if (flags.UpdateDisplayName)
        {
            entry.DisplayName = useAnonymous
                ? reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion))
                : reader.ReadOptional(ReadDelegates.String);
        }
        return entry;
    }

    private static void WriteModernEntry(ref MinecraftPrimitiveWriter writer, ModernEntry entry, PlayerInfoFlags flags, int protocolVersion, bool useAnonymous)
    {
        writer.WriteUUID(entry.Uuid);
        if (flags.AddPlayer)
        {
            writer.WriteGameProfile(entry.Player ?? throw new InvalidOperationException("PlayerInfo player missing."), protocolVersion);
        }
        if (flags.InitializeChat)
        {
            writer.WriteChatSession(entry.ChatSession, protocolVersion);
        }
        if (flags.UpdateGameMode)
        {
            writer.WriteVarInt(entry.GameMode ?? throw new InvalidOperationException("PlayerInfo gamemode missing."));
        }
        if (flags.UpdateListed)
        {
            writer.WriteVarInt(entry.Listed ?? throw new InvalidOperationException("PlayerInfo listed missing."));
        }
        if (flags.UpdateLatency)
        {
            writer.WriteVarInt(entry.Latency ?? throw new InvalidOperationException("PlayerInfo latency missing."));
        }
        if (flags.UpdateDisplayName)
        {
            if (useAnonymous)
            {
                if (entry.DisplayName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag((NbtTag)entry.DisplayName, protocolVersion);
                }
            }
            else
            {
                if (entry.DisplayName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString((string)entry.DisplayName);
                }
            }
        }
    }

    private static byte BuildFlags(PlayerInfoFlags flags)
    {
        byte value = 0;
        if (flags.AddPlayer) value |= 0x01;
        if (flags.InitializeChat) value |= 0x02;
        if (flags.UpdateGameMode) value |= 0x04;
        if (flags.UpdateListed) value |= 0x08;
        if (flags.UpdateLatency) value |= 0x10;
        if (flags.UpdateDisplayName) value |= 0x20;
        return value;
    }

    private static PlayerInfoFlags ReadFlags(byte value)
    {
        return new PlayerInfoFlags
        {
            AddPlayer = (value & 0x01) != 0,
            InitializeChat = (value & 0x02) != 0,
            UpdateGameMode = (value & 0x04) != 0,
            UpdateListed = (value & 0x08) != 0,
            UpdateLatency = (value & 0x10) != 0,
            UpdateDisplayName = (value & 0x20) != 0
        };
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_760Fields
    {
        public LegacyEntry[] Data { get; set; }
    }

    public struct V761_764Fields
    {
        public PlayerInfoFlags Flags { get; set; }
        public ModernEntry[] Data { get; set; }
    }

    public struct V765_LastFields
    {
        public PlayerInfoFlags Flags { get; set; }
        public ModernEntry[] Data { get; set; }
    }

    public struct LegacyEntry
    {
        public Guid Uuid { get; set; }
        public LegacyAddPlayer? AddPlayer { get; set; }
        public int? GameMode { get; set; }
        public int? Ping { get; set; }
        public string? DisplayName { get; set; }
    }

    public struct LegacyAddPlayer
    {
        public string Name { get; set; }
        public PlayerProperty[] Properties { get; set; }
        public int GameMode { get; set; }
        public int Ping { get; set; }
        public string? DisplayName { get; set; }
        public CryptoData? Crypto { get; set; }
    }

    public struct PlayerProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string? Signature { get; set; }
    }

    public struct CryptoData
    {
        public long Timestamp { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] Signature { get; set; }
    }

    public struct ModernEntry
    {
        public Guid Uuid { get; set; }
        public GameProfile? Player { get; set; }
        public ChatSession? ChatSession { get; set; }
        public int? GameMode { get; set; }
        public int? Listed { get; set; }
        public int? Latency { get; set; }
        public object? DisplayName { get; set; }
    }

    public struct PlayerInfoFlags
    {
        public bool AddPlayer { get; set; }
        public bool InitializeChat { get; set; }
        public bool UpdateGameMode { get; set; }
        public bool UpdateListed { get; set; }
        public bool UpdateLatency { get; set; }
        public bool UpdateDisplayName { get; set; }
    }
}
