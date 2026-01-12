using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Map", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class MapPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
        new(755, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public int ItemDamage { get; set; }
    public sbyte Scale { get; set; }
    public bool Locked { get; set; }

    public VFirst_754Fields? VFirst_754 { get; set; }
    public V755_764Fields? V755_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                var fields = VFirst_754 ?? throw new InvalidOperationException("Map VFirst_754 fields missing.");
                writer.WriteVarInt(ItemDamage);
                writer.WriteSignedByte(Scale);
                writer.WriteBoolean(fields.TrackingPosition);
                writer.WriteBoolean(Locked);
                writer.WriteVarInt(fields.Icons.Length);
                for (int i = 0; i < fields.Icons.Length; i++)
                {
                    writer.WriteVarInt(fields.Icons[i].Type);
                    writer.WriteSignedByte(fields.Icons[i].X);
                    writer.WriteSignedByte(fields.Icons[i].Z);
                    writer.WriteUnsignedByte(fields.Icons[i].Direction);
                    if (fields.Icons[i].DisplayName is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteString(fields.Icons[i].DisplayName!);
                    }
                }
                writer.WriteSignedByte(fields.Columns);
                if (fields.Columns != 0)
                {
                    writer.WriteSignedByte(fields.Rows ?? 0);
                    writer.WriteSignedByte(fields.X ?? 0);
                    writer.WriteSignedByte(fields.Y ?? 0);
                    writer.WriteBuffer<VarInt>(fields.Data ?? Array.Empty<byte>());
                }
                return;
            }
            case >= 755 and <= 764:
            {
                var fields = V755_764 ?? throw new InvalidOperationException("Map V755_764 fields missing.");
                writer.WriteVarInt(ItemDamage);
                writer.WriteSignedByte(Scale);
                writer.WriteBoolean(Locked);
                if (fields.Icons is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVarInt(fields.Icons.Length);
                    for (int i = 0; i < fields.Icons.Length; i++)
                    {
                        writer.WriteVarInt(fields.Icons[i].Type);
                        writer.WriteSignedByte(fields.Icons[i].X);
                        writer.WriteSignedByte(fields.Icons[i].Z);
                        writer.WriteUnsignedByte(fields.Icons[i].Direction);
                        if (fields.Icons[i].DisplayName is null)
                        {
                            writer.WriteBoolean(false);
                        }
                        else
                        {
                            writer.WriteBoolean(true);
                            writer.WriteString(fields.Icons[i].DisplayName!);
                        }
                    }
                }
                writer.WriteUnsignedByte(fields.Columns);
                if (fields.Columns != 0)
                {
                    writer.WriteUnsignedByte(fields.Rows ?? 0);
                    writer.WriteUnsignedByte(fields.X ?? 0);
                    writer.WriteUnsignedByte(fields.Y ?? 0);
                    writer.WriteBuffer<VarInt>(fields.Data ?? Array.Empty<byte>());
                }
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("Map V765_Last fields missing.");
                writer.WriteVarInt(ItemDamage);
                writer.WriteSignedByte(Scale);
                writer.WriteBoolean(Locked);
                if (fields.Icons is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVarInt(fields.Icons.Length);
                    for (int i = 0; i < fields.Icons.Length; i++)
                    {
                        writer.WriteVarInt(fields.Icons[i].Type);
                        writer.WriteSignedByte(fields.Icons[i].X);
                        writer.WriteSignedByte(fields.Icons[i].Z);
                        writer.WriteUnsignedByte(fields.Icons[i].Direction);
                        if (fields.Icons[i].DisplayName is null)
                        {
                            writer.WriteBoolean(false);
                        }
                        else
                        {
                            writer.WriteBoolean(true);
                            writer.WriteAnonymousNbtTag(fields.Icons[i].DisplayName!, protocolVersion);
                        }
                    }
                }
                writer.WriteUnsignedByte(fields.Columns);
                if (fields.Columns != 0)
                {
                    writer.WriteUnsignedByte(fields.Rows ?? 0);
                    writer.WriteUnsignedByte(fields.X ?? 0);
                    writer.WriteUnsignedByte(fields.Y ?? 0);
                    writer.WriteBuffer<VarInt>(fields.Data ?? Array.Empty<byte>());
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Map), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                var fields = new VFirst_754Fields();
                ItemDamage = reader.ReadVarInt();
                Scale = reader.ReadSignedByte();
                fields.TrackingPosition = reader.ReadBoolean();
                Locked = reader.ReadBoolean();
                int iconCount = reader.ReadVarInt();
                var icons = new IconEntryString[iconCount];
                for (int i = 0; i < icons.Length; i++)
                {
                    icons[i] = new IconEntryString
                    {
                        Type = reader.ReadVarInt(),
                        X = reader.ReadSignedByte(),
                        Z = reader.ReadSignedByte(),
                        Direction = reader.ReadUnsignedByte(),
                        DisplayName = reader.ReadOptional(ReadDelegates.String)
                    };
                }
                fields.Icons = icons;
                fields.Columns = reader.ReadSignedByte();
                if (fields.Columns != 0)
                {
                    fields.Rows = reader.ReadSignedByte();
                    fields.X = reader.ReadSignedByte();
                    fields.Y = reader.ReadSignedByte();
                    fields.Data = reader.ReadBuffer(LengthFormat.VarInt);
                }
                VFirst_754 = fields;
                return;
            }
            case >= 755 and <= 764:
            {
                var fields = new V755_764Fields();
                ItemDamage = reader.ReadVarInt();
                Scale = reader.ReadSignedByte();
                Locked = reader.ReadBoolean();
                if (reader.ReadBoolean())
                {
                    int iconCount = reader.ReadVarInt();
                    var icons = new IconEntryString[iconCount];
                    for (int i = 0; i < icons.Length; i++)
                    {
                        icons[i] = new IconEntryString
                        {
                            Type = reader.ReadVarInt(),
                            X = reader.ReadSignedByte(),
                            Z = reader.ReadSignedByte(),
                            Direction = reader.ReadUnsignedByte(),
                            DisplayName = reader.ReadOptional(ReadDelegates.String)
                        };
                    }
                    fields.Icons = icons;
                }
                fields.Columns = reader.ReadUnsignedByte();
                if (fields.Columns != 0)
                {
                    fields.Rows = reader.ReadUnsignedByte();
                    fields.X = reader.ReadUnsignedByte();
                    fields.Y = reader.ReadUnsignedByte();
                    fields.Data = reader.ReadBuffer(LengthFormat.VarInt);
                }
                V755_764 = fields;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V765_LastFields();
                ItemDamage = reader.ReadVarInt();
                Scale = reader.ReadSignedByte();
                Locked = reader.ReadBoolean();
                if (reader.ReadBoolean())
                {
                    int iconCount = reader.ReadVarInt();
                    var icons = new IconEntryNbt[iconCount];
                    for (int i = 0; i < icons.Length; i++)
                    {
                        icons[i] = new IconEntryNbt
                        {
                            Type = reader.ReadVarInt(),
                            X = reader.ReadSignedByte(),
                            Z = reader.ReadSignedByte(),
                            Direction = reader.ReadUnsignedByte(),
                            DisplayName = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion))
                        };
                    }
                    fields.Icons = icons;
                }
                fields.Columns = reader.ReadUnsignedByte();
                if (fields.Columns != 0)
                {
                    fields.Rows = reader.ReadUnsignedByte();
                    fields.X = reader.ReadUnsignedByte();
                    fields.Y = reader.ReadUnsignedByte();
                    fields.Data = reader.ReadBuffer(LengthFormat.VarInt);
                }
                V765_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Map), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_754Fields
    {
        public bool TrackingPosition { get; set; }
        public IconEntryString[] Icons { get; set; }
        public sbyte Columns { get; set; }
        public sbyte? Rows { get; set; }
        public sbyte? X { get; set; }
        public sbyte? Y { get; set; }
        public byte[]? Data { get; set; }
    }

    public struct V755_764Fields
    {
        public IconEntryString[]? Icons { get; set; }
        public byte Columns { get; set; }
        public byte? Rows { get; set; }
        public byte? X { get; set; }
        public byte? Y { get; set; }
        public byte[]? Data { get; set; }
    }

    public struct V765_LastFields
    {
        public IconEntryNbt[]? Icons { get; set; }
        public byte Columns { get; set; }
        public byte? Rows { get; set; }
        public byte? X { get; set; }
        public byte? Y { get; set; }
        public byte[]? Data { get; set; }
    }

    public struct IconEntryString
    {
        public int Type { get; set; }
        public sbyte X { get; set; }
        public sbyte Z { get; set; }
        public byte Direction { get; set; }
        public string? DisplayName { get; set; }
    }

    public struct IconEntryNbt
    {
        public int Type { get; set; }
        public sbyte X { get; set; }
        public sbyte Z { get; set; }
        public byte Direction { get; set; }
        public NbtTag? DisplayName { get; set; }
    }
}
