using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Union]
public partial record TeamAction
{
    partial record CreatedVUntil764(string Name, sbyte FriendlyFire, string NameTagVisibility, string CollisionRule, int Formatting, string Prefix, string Suffix, string[] Players);
    partial record Removed();
    partial record UpdatedVUntil764(string Name, sbyte FriendlyFire, string NameTagVisibility, string CollisionRule, int Formatting, string Prefix, string Suffix);
    partial record PlayersAdded(string[] Players);
    partial record PlayersRemoved(string[] Players);
    partial record CreatedV771_775(NbtTag Name, TeamFlags Flags, int NameTagVisibility, int CollisionRule, int Formatting, NbtTag Prefix, NbtTag Suffix, string[] Players);
    partial record UpdatedV771_775(NbtTag Name, TeamFlags Flags, int NameTagVisibility, int CollisionRule, int Formatting, NbtTag Prefix, NbtTag Suffix);
    partial record PlayersChanged(string[] Players);
    partial record CreatedV776_Last(NbtTag Name, NbtTag Prefix, NbtTag Suffix, int NameTagVisibility, int CollisionRule, int? Formatting, TeamFlags Flags, string[] Players);
    partial record UpdatedV776_Last(NbtTag Name, NbtTag Prefix, NbtTag Suffix, int NameTagVisibility, int CollisionRule, int? Formatting, TeamFlags Flags);
    public static TeamAction Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeamAction>(protocolVersion);
        if (protocolVersion <= 764)
        {
            switch (discriminator)
            {
                case 0:
                {
                    var name = reader.ReadString();
                    var friendlyFire = reader.ReadSignedByte();
                    var nameTagVisibility = reader.ReadString();
                    var collisionRule = reader.ReadString();
                    var formatting = reader.ReadVarInt();
                    var prefix = reader.ReadString();
                    var suffix = reader.ReadString();
                    int playersCount = reader.ReadVarInt();
                    var players = new string[playersCount];
                    for (int i = 0; i < players.Length; i++)
                        players[i] = reader.ReadString();
                    return new CreatedVUntil764(name, friendlyFire, nameTagVisibility, collisionRule, formatting, prefix, suffix, players);
                }

                case 1:
                {
                    return new Removed();
                }

                case 2:
                {
                    var name = reader.ReadString();
                    var friendlyFire = reader.ReadSignedByte();
                    var nameTagVisibility = reader.ReadString();
                    var collisionRule = reader.ReadString();
                    var formatting = reader.ReadVarInt();
                    var prefix = reader.ReadString();
                    var suffix = reader.ReadString();
                    return new UpdatedVUntil764(name, friendlyFire, nameTagVisibility, collisionRule, formatting, prefix, suffix);
                }

                case 3:
                {
                    int playersCount = reader.ReadVarInt();
                    var players = new string[playersCount];
                    for (int i = 0; i < players.Length; i++)
                        players[i] = reader.ReadString();
                    return new PlayersAdded(players);
                }

                case 4:
                {
                    int playersCount = reader.ReadVarInt();
                    var players = new string[playersCount];
                    for (int i = 0; i < players.Length; i++)
                        players[i] = reader.ReadString();
                    return new PlayersRemoved(players);
                }
            }

            throw new System.NotSupportedException($"TeamAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
        }

        if (protocolVersion >= 771 && protocolVersion <= 775)
        {
            switch (discriminator)
            {
                case 0:
                {
                    var name = reader.ReadNbtTag(false)!;
                    var flags = reader.ReadType<TeamFlags>(protocolVersion);
                    var nameTagVisibility = reader.ReadVarInt();
                    var collisionRule = reader.ReadVarInt();
                    var formatting = reader.ReadVarInt();
                    var prefix = reader.ReadNbtTag(false)!;
                    var suffix = reader.ReadNbtTag(false)!;
                    int playersCount = reader.ReadVarInt();
                    var players = new string[playersCount];
                    for (int i = 0; i < players.Length; i++)
                        players[i] = reader.ReadString();
                    return new CreatedV771_775(name, flags, nameTagVisibility, collisionRule, formatting, prefix, suffix, players);
                }

                case 1:
                {
                    return new Removed();
                }

                case 2:
                {
                    var name = reader.ReadNbtTag(false)!;
                    var flags = reader.ReadType<TeamFlags>(protocolVersion);
                    var nameTagVisibility = reader.ReadVarInt();
                    var collisionRule = reader.ReadVarInt();
                    var formatting = reader.ReadVarInt();
                    var prefix = reader.ReadNbtTag(false)!;
                    var suffix = reader.ReadNbtTag(false)!;
                    return new UpdatedV771_775(name, flags, nameTagVisibility, collisionRule, formatting, prefix, suffix);
                }

                case 3:
                case 4:
                {
                    int playersCount = reader.ReadVarInt();
                    var players = new string[playersCount];
                    for (int i = 0; i < players.Length; i++)
                        players[i] = reader.ReadString();
                    return new PlayersChanged(players);
                }
            }

            throw new System.NotSupportedException($"TeamAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
        }

        if (protocolVersion >= 776)
        {
            switch (discriminator)
            {
                case 0:
                {
                    var name = reader.ReadNbtTag(false)!;
                    var prefix = reader.ReadNbtTag(false)!;
                    var suffix = reader.ReadNbtTag(false)!;
                    var nameTagVisibility = reader.ReadVarInt();
                    var collisionRule = reader.ReadVarInt();
                    int? formatting = null;
                    if (reader.ReadBoolean())
                        formatting = reader.ReadVarInt();
                    var flags = reader.ReadType<TeamFlags>(protocolVersion);
                    int playersCount = reader.ReadVarInt();
                    var players = new string[playersCount];
                    for (int i = 0; i < players.Length; i++)
                        players[i] = reader.ReadString();
                    return new CreatedV776_Last(name, prefix, suffix, nameTagVisibility, collisionRule, formatting, flags, players);
                }

                case 1:
                {
                    return new Removed();
                }

                case 2:
                {
                    var name = reader.ReadNbtTag(false)!;
                    var prefix = reader.ReadNbtTag(false)!;
                    var suffix = reader.ReadNbtTag(false)!;
                    var nameTagVisibility = reader.ReadVarInt();
                    var collisionRule = reader.ReadVarInt();
                    int? formatting = null;
                    if (reader.ReadBoolean())
                        formatting = reader.ReadVarInt();
                    var flags = reader.ReadType<TeamFlags>(protocolVersion);
                    return new UpdatedV776_Last(name, prefix, suffix, nameTagVisibility, collisionRule, formatting, flags);
                }

                case 3:
                case 4:
                {
                    int playersCount = reader.ReadVarInt();
                    var players = new string[playersCount];
                    for (int i = 0; i < players.Length; i++)
                        players[i] = reader.ReadString();
                    return new PlayersChanged(players);
                }
            }

            throw new System.NotSupportedException($"TeamAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
        }

        throw new System.NotSupportedException($"TeamAction has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeamAction>(protocolVersion);
        if (protocolVersion <= 764)
        {
            switch (this)
            {
                case CreatedVUntil764 arm:
                {
                    string Name = arm.Name;
                    sbyte FriendlyFire = arm.FriendlyFire;
                    string NameTagVisibility = arm.NameTagVisibility;
                    string CollisionRule = arm.CollisionRule;
                    int Formatting = arm.Formatting;
                    string Prefix = arm.Prefix;
                    string Suffix = arm.Suffix;
                    string[] Players = arm.Players;
                    writer.WriteString(Name);
                    writer.WriteSignedByte(FriendlyFire);
                    writer.WriteString(NameTagVisibility);
                    writer.WriteString(CollisionRule);
                    writer.WriteVarInt(Formatting);
                    writer.WriteString(Prefix);
                    writer.WriteString(Suffix);
                    writer.WriteVarInt(Players.Length);
                    foreach (var playersItem in Players)
                        writer.WriteString(playersItem);
                    return;
                }

                case Removed _:
                {
                    return;
                }

                case UpdatedVUntil764 arm:
                {
                    string Name = arm.Name;
                    sbyte FriendlyFire = arm.FriendlyFire;
                    string NameTagVisibility = arm.NameTagVisibility;
                    string CollisionRule = arm.CollisionRule;
                    int Formatting = arm.Formatting;
                    string Prefix = arm.Prefix;
                    string Suffix = arm.Suffix;
                    writer.WriteString(Name);
                    writer.WriteSignedByte(FriendlyFire);
                    writer.WriteString(NameTagVisibility);
                    writer.WriteString(CollisionRule);
                    writer.WriteVarInt(Formatting);
                    writer.WriteString(Prefix);
                    writer.WriteString(Suffix);
                    return;
                }

                case PlayersAdded arm:
                {
                    string[] Players = arm.Players;
                    writer.WriteVarInt(Players.Length);
                    foreach (var playersItem in Players)
                        writer.WriteString(playersItem);
                    return;
                }

                case PlayersRemoved arm:
                {
                    string[] Players = arm.Players;
                    writer.WriteVarInt(Players.Length);
                    foreach (var playersItem in Players)
                        writer.WriteString(playersItem);
                    return;
                }
            }

            throw new System.NotSupportedException($"TeamAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        if (protocolVersion >= 771 && protocolVersion <= 775)
        {
            switch (this)
            {
                case CreatedV771_775 arm:
                {
                    NbtTag Name = arm.Name;
                    TeamFlags Flags = arm.Flags;
                    int NameTagVisibility = arm.NameTagVisibility;
                    int CollisionRule = arm.CollisionRule;
                    int Formatting = arm.Formatting;
                    NbtTag Prefix = arm.Prefix;
                    NbtTag Suffix = arm.Suffix;
                    string[] Players = arm.Players;
                    writer.WriteNbt(Name);
                    writer.WriteType<TeamFlags>(Flags, protocolVersion);
                    writer.WriteVarInt(NameTagVisibility);
                    writer.WriteVarInt(CollisionRule);
                    writer.WriteVarInt(Formatting);
                    writer.WriteNbt(Prefix);
                    writer.WriteNbt(Suffix);
                    writer.WriteVarInt(Players.Length);
                    foreach (var playersItem in Players)
                        writer.WriteString(playersItem);
                    return;
                }

                case Removed _:
                {
                    return;
                }

                case UpdatedV771_775 arm:
                {
                    NbtTag Name = arm.Name;
                    TeamFlags Flags = arm.Flags;
                    int NameTagVisibility = arm.NameTagVisibility;
                    int CollisionRule = arm.CollisionRule;
                    int Formatting = arm.Formatting;
                    NbtTag Prefix = arm.Prefix;
                    NbtTag Suffix = arm.Suffix;
                    writer.WriteNbt(Name);
                    writer.WriteType<TeamFlags>(Flags, protocolVersion);
                    writer.WriteVarInt(NameTagVisibility);
                    writer.WriteVarInt(CollisionRule);
                    writer.WriteVarInt(Formatting);
                    writer.WriteNbt(Prefix);
                    writer.WriteNbt(Suffix);
                    return;
                }

                case PlayersChanged arm:
                {
                    string[] Players = arm.Players;
                    writer.WriteVarInt(Players.Length);
                    foreach (var playersItem in Players)
                        writer.WriteString(playersItem);
                    return;
                }
            }

            throw new System.NotSupportedException($"TeamAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        if (protocolVersion >= 776)
        {
            switch (this)
            {
                case CreatedV776_Last arm:
                {
                    NbtTag Name = arm.Name;
                    NbtTag Prefix = arm.Prefix;
                    NbtTag Suffix = arm.Suffix;
                    int NameTagVisibility = arm.NameTagVisibility;
                    int CollisionRule = arm.CollisionRule;
                    int? Formatting = arm.Formatting;
                    TeamFlags Flags = arm.Flags;
                    string[] Players = arm.Players;
                    writer.WriteNbt(Name);
                    writer.WriteNbt(Prefix);
                    writer.WriteNbt(Suffix);
                    writer.WriteVarInt(NameTagVisibility);
                    writer.WriteVarInt(CollisionRule);
                    writer.WriteBoolean(Formatting is not null);
                    if (Formatting is { } formattingValue)
                        writer.WriteVarInt(formattingValue);
                    writer.WriteType<TeamFlags>(Flags, protocolVersion);
                    writer.WriteVarInt(Players.Length);
                    foreach (var playersItem in Players)
                        writer.WriteString(playersItem);
                    return;
                }

                case Removed _:
                {
                    return;
                }

                case UpdatedV776_Last arm:
                {
                    NbtTag Name = arm.Name;
                    NbtTag Prefix = arm.Prefix;
                    NbtTag Suffix = arm.Suffix;
                    int NameTagVisibility = arm.NameTagVisibility;
                    int CollisionRule = arm.CollisionRule;
                    int? Formatting = arm.Formatting;
                    TeamFlags Flags = arm.Flags;
                    writer.WriteNbt(Name);
                    writer.WriteNbt(Prefix);
                    writer.WriteNbt(Suffix);
                    writer.WriteVarInt(NameTagVisibility);
                    writer.WriteVarInt(CollisionRule);
                    writer.WriteBoolean(Formatting is not null);
                    if (Formatting is { } formattingValue)
                        writer.WriteVarInt(formattingValue);
                    writer.WriteType<TeamFlags>(Flags, protocolVersion);
                    return;
                }

                case PlayersChanged arm:
                {
                    string[] Players = arm.Players;
                    writer.WriteVarInt(Players.Length);
                    foreach (var playersItem in Players)
                        writer.WriteString(playersItem);
                    return;
                }
            }

            throw new System.NotSupportedException($"TeamAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        throw new System.NotSupportedException($"TeamAction has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        if (protocolVersion <= 764)
        {
            switch (this)
            {
                case CreatedVUntil764 _:
                    return 0;
                case Removed _:
                    return 1;
                case UpdatedVUntil764 _:
                    return 2;
                case PlayersAdded _:
                    return 3;
                case PlayersRemoved _:
                    return 4;
            }

            throw new System.NotSupportedException($"TeamAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        if (protocolVersion >= 771 && protocolVersion <= 775)
        {
            switch (this)
            {
                case CreatedV771_775 _:
                    return 0;
                case Removed _:
                    return 1;
                case UpdatedV771_775 _:
                    return 2;
                case PlayersChanged _:
                    return 3;
            }

            throw new System.NotSupportedException($"TeamAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        if (protocolVersion >= 776)
        {
            switch (this)
            {
                case CreatedV776_Last _:
                    return 0;
                case Removed _:
                    return 1;
                case UpdatedV776_Last _:
                    return 2;
                case PlayersChanged _:
                    return 3;
            }

            throw new System.NotSupportedException($"TeamAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        throw new System.NotSupportedException($"TeamAction has no wire layout for protocol version {protocolVersion}.");
    }
}
