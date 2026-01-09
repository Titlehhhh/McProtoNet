using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Teams", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class TeamsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, 769),
        new(770, 770),
        new(771, MinecraftVersion.LatestProtocol),
    };

    public string Team { get; set; } = string.Empty;
    public sbyte Mode { get; set; }

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_769Fields? V765_769 { get; set; }
    public V770Fields? V770 { get; set; }
    public V771_LastFields? V771_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("Teams VFirst_764 fields missing.");
                writer.WriteString(Team);
                writer.WriteSignedByte(Mode);
                if (Mode is 0 or 2)
                {
                    writer.WriteString(fields.Name ?? throw new InvalidOperationException("Teams name missing."));
                    writer.WriteSignedByte(fields.FriendlyFire ?? throw new InvalidOperationException("Teams friendlyFire missing."));
                    writer.WriteString(fields.NameTagVisibility ?? throw new InvalidOperationException("Teams nameTagVisibility missing."));
                    writer.WriteString(fields.CollisionRule ?? throw new InvalidOperationException("Teams collisionRule missing."));
                    writer.WriteVarInt(fields.Formatting ?? throw new InvalidOperationException("Teams formatting missing."));
                    writer.WriteString(fields.Prefix ?? throw new InvalidOperationException("Teams prefix missing."));
                    writer.WriteString(fields.Suffix ?? throw new InvalidOperationException("Teams suffix missing."));
                }
                if (Mode is 0 or 3 or 4)
                {
                    WritePlayers(ref writer, fields.Players);
                }
                return;
            }
            case >= 765 and <= 769:
            {
                var fields = V765_769 ?? throw new InvalidOperationException("Teams V765_769 fields missing.");
                writer.WriteString(Team);
                writer.WriteSignedByte(Mode);
                if (Mode is 0 or 2)
                {
                    writer.WriteAnonymousNbtTag(fields.Name ?? throw new InvalidOperationException("Teams name missing."), protocolVersion);
                    writer.WriteSignedByte(fields.FriendlyFire ?? throw new InvalidOperationException("Teams friendlyFire missing."));
                    writer.WriteString(fields.NameTagVisibility ?? throw new InvalidOperationException("Teams nameTagVisibility missing."));
                    writer.WriteString(fields.CollisionRule ?? throw new InvalidOperationException("Teams collisionRule missing."));
                    writer.WriteVarInt(fields.Formatting ?? throw new InvalidOperationException("Teams formatting missing."));
                    writer.WriteAnonymousNbtTag(fields.Prefix ?? throw new InvalidOperationException("Teams prefix missing."), protocolVersion);
                    writer.WriteAnonymousNbtTag(fields.Suffix ?? throw new InvalidOperationException("Teams suffix missing."), protocolVersion);
                }
                if (Mode is 0 or 3 or 4)
                {
                    WritePlayers(ref writer, fields.Players);
                }
                return;
            }
            case 770:
            {
                var fields = V770 ?? throw new InvalidOperationException("Teams V770 fields missing.");
                writer.WriteString(Team);
                writer.WriteSignedByte(Mode);
                if (Mode is 0 or 2)
                {
                    writer.WriteAnonymousNbtTag(fields.Name ?? throw new InvalidOperationException("Teams name missing."), protocolVersion);
                    writer.WriteSignedByte(fields.FriendlyFire ?? throw new InvalidOperationException("Teams friendlyFire missing."));
                    writer.WriteVarInt(fields.NameTagVisibility ?? throw new InvalidOperationException("Teams nameTagVisibility missing."));
                    writer.WriteVarInt(fields.CollisionRule ?? throw new InvalidOperationException("Teams collisionRule missing."));
                    writer.WriteVarInt(fields.Formatting ?? throw new InvalidOperationException("Teams formatting missing."));
                    writer.WriteAnonymousNbtTag(fields.Prefix ?? throw new InvalidOperationException("Teams prefix missing."), protocolVersion);
                    writer.WriteAnonymousNbtTag(fields.Suffix ?? throw new InvalidOperationException("Teams suffix missing."), protocolVersion);
                }
                if (Mode is 0 or 3 or 4)
                {
                    WritePlayers(ref writer, fields.Players);
                }
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V771_Last ?? throw new InvalidOperationException("Teams V771_Last fields missing.");
                writer.WriteString(Team);
                writer.WriteSignedByte(Mode);
                if (Mode is 0 or 2)
                {
                    writer.WriteAnonymousNbtTag(fields.Name ?? throw new InvalidOperationException("Teams name missing."), protocolVersion);
                    byte flags = 0;
                    if (fields.FriendlyFire) flags |= 0x01;
                    if (fields.SeeFriendlyInvisible) flags |= 0x02;
                    writer.WriteUnsignedByte(flags);
                    writer.WriteVarInt(fields.NameTagVisibility ?? throw new InvalidOperationException("Teams nameTagVisibility missing."));
                    writer.WriteVarInt(fields.CollisionRule ?? throw new InvalidOperationException("Teams collisionRule missing."));
                    writer.WriteVarInt(fields.Formatting ?? throw new InvalidOperationException("Teams formatting missing."));
                    writer.WriteAnonymousNbtTag(fields.Prefix ?? throw new InvalidOperationException("Teams prefix missing."), protocolVersion);
                    writer.WriteAnonymousNbtTag(fields.Suffix ?? throw new InvalidOperationException("Teams suffix missing."), protocolVersion);
                }
                if (Mode is 0 or 3 or 4)
                {
                    WritePlayers(ref writer, fields.Players);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Teams), protocolVersion, SupportedVersionsStatic);
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
                Team = reader.ReadString();
                Mode = reader.ReadSignedByte();
                if (Mode is 0 or 2)
                {
                    fields.Name = reader.ReadString();
                    fields.FriendlyFire = reader.ReadSignedByte();
                    fields.NameTagVisibility = reader.ReadString();
                    fields.CollisionRule = reader.ReadString();
                    fields.Formatting = reader.ReadVarInt();
                    fields.Prefix = reader.ReadString();
                    fields.Suffix = reader.ReadString();
                }
                if (Mode is 0 or 3 or 4)
                {
                    fields.Players = ReadPlayers(ref reader);
                }
                VFirst_764 = fields;
                return;
            }
            case >= 765 and <= 769:
            {
                var fields = new V765_769Fields();
                Team = reader.ReadString();
                Mode = reader.ReadSignedByte();
                if (Mode is 0 or 2)
                {
                    fields.Name = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("Teams name missing.");
                    fields.FriendlyFire = reader.ReadSignedByte();
                    fields.NameTagVisibility = reader.ReadString();
                    fields.CollisionRule = reader.ReadString();
                    fields.Formatting = reader.ReadVarInt();
                    fields.Prefix = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("Teams prefix missing.");
                    fields.Suffix = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("Teams suffix missing.");
                }
                if (Mode is 0 or 3 or 4)
                {
                    fields.Players = ReadPlayers(ref reader);
                }
                V765_769 = fields;
                return;
            }
            case 770:
            {
                var fields = new V770Fields();
                Team = reader.ReadString();
                Mode = reader.ReadSignedByte();
                if (Mode is 0 or 2)
                {
                    fields.Name = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("Teams name missing.");
                    fields.FriendlyFire = reader.ReadSignedByte();
                    fields.NameTagVisibility = reader.ReadVarInt();
                    fields.CollisionRule = reader.ReadVarInt();
                    fields.Formatting = reader.ReadVarInt();
                    fields.Prefix = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("Teams prefix missing.");
                    fields.Suffix = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("Teams suffix missing.");
                }
                if (Mode is 0 or 3 or 4)
                {
                    fields.Players = ReadPlayers(ref reader);
                }
                V770 = fields;
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V771_LastFields();
                Team = reader.ReadString();
                Mode = reader.ReadSignedByte();
                if (Mode is 0 or 2)
                {
                    fields.Name = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("Teams name missing.");
                    byte flags = reader.ReadUnsignedByte();
                    fields.FriendlyFire = (flags & 0x01) != 0;
                    fields.SeeFriendlyInvisible = (flags & 0x02) != 0;
                    fields.NameTagVisibility = reader.ReadVarInt();
                    fields.CollisionRule = reader.ReadVarInt();
                    fields.Formatting = reader.ReadVarInt();
                    fields.Prefix = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("Teams prefix missing.");
                    fields.Suffix = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("Teams suffix missing.");
                }
                if (Mode is 0 or 3 or 4)
                {
                    fields.Players = ReadPlayers(ref reader);
                }
                V771_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Teams), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static string[] ReadPlayers(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<string>();
        }

        var players = new string[count];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = reader.ReadString();
        }
        return players;
    }

    private static void WritePlayers(ref MinecraftPrimitiveWriter writer, string[]? players)
    {
        if (players is null)
        {
            writer.WriteVarInt(0);
            return;
        }

        writer.WriteVarInt(players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            writer.WriteString(players[i]);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_764Fields
    {
        public string? Name { get; set; }
        public sbyte? FriendlyFire { get; set; }
        public string? NameTagVisibility { get; set; }
        public string? CollisionRule { get; set; }
        public int? Formatting { get; set; }
        public string? Prefix { get; set; }
        public string? Suffix { get; set; }
        public string[]? Players { get; set; }
    }

    public struct V765_769Fields
    {
        public NbtTag? Name { get; set; }
        public sbyte? FriendlyFire { get; set; }
        public string? NameTagVisibility { get; set; }
        public string? CollisionRule { get; set; }
        public int? Formatting { get; set; }
        public NbtTag? Prefix { get; set; }
        public NbtTag? Suffix { get; set; }
        public string[]? Players { get; set; }
    }

    public struct V770Fields
    {
        public NbtTag? Name { get; set; }
        public sbyte? FriendlyFire { get; set; }
        public int? NameTagVisibility { get; set; }
        public int? CollisionRule { get; set; }
        public int? Formatting { get; set; }
        public NbtTag? Prefix { get; set; }
        public NbtTag? Suffix { get; set; }
        public string[]? Players { get; set; }
    }

    public struct V771_LastFields
    {
        public NbtTag? Name { get; set; }
        public bool FriendlyFire { get; set; }
        public bool SeeFriendlyInvisible { get; set; }
        public int? NameTagVisibility { get; set; }
        public int? CollisionRule { get; set; }
        public int? Formatting { get; set; }
        public NbtTag? Prefix { get; set; }
        public NbtTag? Suffix { get; set; }
        public string[]? Players { get; set; }
    }
}
