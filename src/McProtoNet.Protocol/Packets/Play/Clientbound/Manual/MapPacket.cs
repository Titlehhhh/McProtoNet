namespace McProtoNet.Protocol.Packets.Play.Clientbound;

using McProtoNet.NBT;
using McProtoNet.Serialization;

[PacketInfo("Map", PacketState.Play, PacketDirection.Clientbound)]
public abstract partial class MapPacket : IServerPacket
{
    public int ItemDamage { get; set; }
    public sbyte Scale { get; set; }
    public byte[] Data { get; set; }

    public struct MapIcon
    {
        public int Type { get; set; }
        public sbyte X { get; set; }
        public sbyte Z { get; set; }
        public byte Direction { get; set; }
        public string? DisplayName { get; set; }
        public NbtTag? DisplayNameNbt { get; set; }
    }

    public struct MapIconLegacy
    {
        public sbyte DirectionAndType { get; set; }
        public sbyte X { get; set; }
        public sbyte Z { get; set; }
    }

    [PacketSubInfo(340, 351)]
    public sealed partial class V340_351 : MapPacket
    {
        public bool TrackingPosition { get; set; }
        public MapIconLegacy[] Icons { get; set; }
        public sbyte Columns { get; set; }
        public sbyte? Rows { get; set; }
        public sbyte? X { get; set; }
        public sbyte? Y { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ItemDamage = reader.ReadVarInt();
            Scale = reader.ReadSignedByte();
            TrackingPosition = reader.ReadBoolean();
            Icons = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => new MapIconLegacy
            {
                DirectionAndType = r.ReadSignedByte(),
                X = r.ReadSignedByte(),
                Z = r.ReadSignedByte()
            });
            Columns = reader.ReadSignedByte();
            if (Columns > 0)
            {
                Rows = reader.ReadSignedByte();
                X = reader.ReadSignedByte();
                Y = reader.ReadSignedByte();
                Data = reader.ReadBuffer(LengthFormat.VarInt);
            }
        }
    }

    [PacketSubInfo(393, 404)]
    public sealed partial class V393_404 : MapPacket
    {
        public bool TrackingPosition { get; set; }
        public MapIcon[] Icons { get; set; }
        public sbyte Columns { get; set; }
        public sbyte? Rows { get; set; }
        public sbyte? X { get; set; }
        public sbyte? Y { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ItemDamage = reader.ReadVarInt();
            Scale = reader.ReadSignedByte();
            TrackingPosition = reader.ReadBoolean();
            Icons = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => new MapIcon
            {
                Type = r.ReadVarInt(),
                X = r.ReadSignedByte(),
                Z = r.ReadSignedByte(),
                Direction = r.ReadUnsignedByte(),
                DisplayName = r.ReadOptional((ref MinecraftPrimitiveReader r2) => r2.ReadString())
            });
            Columns = reader.ReadSignedByte();
            if (Columns > 0)
            {
                Rows = reader.ReadSignedByte();
                X = reader.ReadSignedByte();
                Y = reader.ReadSignedByte();
                Data = reader.ReadBuffer(LengthFormat.VarInt);
            }
        }
    }

    [PacketSubInfo(477, 754)]
    public sealed partial class V477_754 : MapPacket
    {
        public bool TrackingPosition { get; set; }
        public bool Locked { get; set; }
        public MapIcon[] Icons { get; set; }
        public sbyte Columns { get; set; }
        public sbyte? Rows { get; set; }
        public sbyte? X { get; set; }
        public sbyte? Y { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ItemDamage = reader.ReadVarInt();
            Scale = reader.ReadSignedByte();
            TrackingPosition = reader.ReadBoolean();
            Locked = reader.ReadBoolean();
            Icons = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => new MapIcon
            {
                Type = r.ReadVarInt(),
                X = r.ReadSignedByte(),
                Z = r.ReadSignedByte(),
                Direction = r.ReadUnsignedByte(),
                DisplayName = r.ReadOptional((ref MinecraftPrimitiveReader r2) => r2.ReadString())
            });
            Columns = reader.ReadSignedByte();
            if (Columns > 0)
            {
                Rows = reader.ReadSignedByte();
                X = reader.ReadSignedByte();
                Y = reader.ReadSignedByte();
                Data = reader.ReadBuffer(LengthFormat.VarInt);
            }
        }
    }

    [PacketSubInfo(755, 764)]
    public sealed partial class V755_764 : MapPacket
    {
        public bool Locked { get; set; }
        public MapIcon[]? Icons { get; set; }
        public byte Columns { get; set; }
        public byte? Rows { get; set; }
        public byte? X { get; set; }
        public byte? Y { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ItemDamage = reader.ReadVarInt();
            Scale = reader.ReadSignedByte();
            Locked = reader.ReadBoolean();
            Icons = reader.ReadOptional((ref MinecraftPrimitiveReader r) =>
                r.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r2) => new MapIcon
                {
                    Type = r2.ReadVarInt(),
                    X = r2.ReadSignedByte(),
                    Z = r2.ReadSignedByte(),
                    Direction = r2.ReadUnsignedByte(),
                    DisplayName = r2.ReadOptional((ref MinecraftPrimitiveReader r3) => r3.ReadString())
                }));
            Columns = reader.ReadUnsignedByte();
            if (Columns > 0)
            {
                Rows = reader.ReadUnsignedByte();
                X = reader.ReadUnsignedByte();
                Y = reader.ReadUnsignedByte();
                Data = reader.ReadBuffer(LengthFormat.VarInt);
            }
        }
    }

    [PacketSubInfo(765, 769)]
    public sealed partial class V765_769 : MapPacket
    {
        public bool Locked { get; set; }
        public MapIcon[]? Icons { get; set; }
        public byte Columns { get; set; }
        public byte? Rows { get; set; }
        public byte? X { get; set; }
        public byte? Y { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ItemDamage = reader.ReadVarInt();
            Scale = reader.ReadSignedByte();
            Locked = reader.ReadBoolean();
            Icons = reader.ReadOptional((ref MinecraftPrimitiveReader r) =>
                r.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r2) => new MapIcon
                {
                    Type = r2.ReadVarInt(),
                    X = r2.ReadSignedByte(),
                    Z = r2.ReadSignedByte(),
                    Direction = r2.ReadUnsignedByte(),
                    DisplayNameNbt = r2.ReadNbtTag(false)
                }));
            Columns = reader.ReadUnsignedByte();
            if (Columns > 0)
            {
                Rows = reader.ReadUnsignedByte();
                X = reader.ReadUnsignedByte();
                Y = reader.ReadUnsignedByte();
                Data = reader.ReadBuffer(LengthFormat.VarInt);
            }
        }
    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
}

