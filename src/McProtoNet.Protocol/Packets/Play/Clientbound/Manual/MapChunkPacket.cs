using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("MapChunk", PacketState.Play, PacketDirection.Clientbound)]
public abstract partial class MapChunkPacket : IServerPacket
{
    public int X { get; set; }
    public int Z { get; set; }
    public byte[] ChunkData { get; set; }

    [PacketSubInfo(340, 404)]
    public sealed partial class V340_404 : MapChunkPacket
    {
        public bool GroundUp { get; set; }
        public int BitMap { get; set; }
        public NbtTag[] BlockEntities { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            X = reader.ReadSignedInt();
            Z = reader.ReadSignedInt();
            GroundUp = reader.ReadBoolean();
            BitMap = reader.ReadVarInt();
            ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
            BlockEntities = reader.ReadArray(LengthFormat.VarInt,
                (ref MinecraftPrimitiveReader r1) => r1.ReadNbtTag(protocolVersion));
        }
    }

    [PacketSubInfo(477, 498)]
    public sealed partial class V477_498 : MapChunkPacket
    {
        public bool GroundUp { get; set; }
        public int BitMap { get; set; }
        public NbtTag Heightmaps { get; set; }
        public NbtTag[] BlockEntities { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            X = reader.ReadSignedInt();
            Z = reader.ReadSignedInt();
            GroundUp = reader.ReadBoolean();
            BitMap = reader.ReadVarInt();
            Heightmaps = reader.ReadNbtTag(protocolVersion);
            ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
            BlockEntities = reader.ReadArray(LengthFormat.VarInt,
                (ref MinecraftPrimitiveReader r1) => r1.ReadNbtTag(true));
        }
    }

    [PacketSubInfo(573, 710)]
    public sealed partial class V573_710 : MapChunkPacket
    {
        public bool GroundUp { get; set; }
        public int BitMap { get; set; }
        public NbtTag Heightmaps { get; set; }

        public int[]? Biomes { get; set; }

        public NbtTag?[] BlockEntities { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            X = reader.ReadSignedInt();
            Z = reader.ReadSignedInt();
            GroundUp = reader.ReadBoolean();
            BitMap = reader.ReadVarInt();
            Heightmaps = reader.ReadNbtTag(protocolVersion);
            if (GroundUp)
            {
                Biomes = reader.ReadArrayInt32BigEndian(1024);
            }

            ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
            BlockEntities = reader.ReadArray(LengthFormat.VarInt,
                (ref MinecraftPrimitiveReader r1) => r1.ReadNbtTag(true));
        }
    }

    [PacketSubInfo(734, 736)]
    public sealed partial class V734_736 : MapChunkPacket
    {
        public bool GroundUp { get; set; }
        public bool IgnoreOldData { get; set; }
        public int BitMap { get; set; }
        public NbtTag Heightmaps { get; set; }
        public int[]? Biomes { get; set; }

        public NbtTag?[] BlockEntities { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            X = reader.ReadSignedInt();
            Z = reader.ReadSignedInt();
            GroundUp = reader.ReadBoolean();
            IgnoreOldData = reader.ReadBoolean();
            BitMap = reader.ReadVarInt();
            Heightmaps = reader.ReadNbtTag(protocolVersion);
            if (GroundUp)
            {
                Biomes = reader.ReadArrayInt32BigEndian(1024);
            }

            ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
            BlockEntities = reader.ReadArray(LengthFormat.VarInt,
                (ref MinecraftPrimitiveReader r1) => r1.ReadNbtTag(true));
        }
    }

    [PacketSubInfo(751, 754)]
    public sealed partial class V751_754 : MapChunkPacket
    {
        public bool GroundUp { get; set; }
        public int BitMap { get; set; }
        public NbtTag Heightmaps { get; set; }
        public int[]? Biomes { get; set; }

        public NbtTag?[] BlockEntities { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            X = reader.ReadSignedInt();
            Z = reader.ReadSignedInt();
            GroundUp = reader.ReadBoolean();
            BitMap = reader.ReadVarInt();
            Heightmaps = reader.ReadNbtTag(protocolVersion);
            if (GroundUp)
            {
                Biomes = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.VarInt);
            }

            ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
            BlockEntities = reader.ReadArray(LengthFormat.VarInt,
                (ref MinecraftPrimitiveReader r1) => r1.ReadNbtTag(true));
        }
    }


    [PacketSubInfo(755, 756)]
    public sealed partial class V755_756 : MapChunkPacket
    {
        public long[] BitMap { get; set; }
        public NbtTag Heightmaps { get; set; }
        public int[]? Biomes { get; set; }

        public NbtTag[] BlockEntities { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            X = reader.ReadSignedInt();
            Z = reader.ReadSignedInt();
            BitMap = reader.ReadArrayInt64BigEndian(reader.ReadVarInt());
            Heightmaps = reader.ReadNbtTag(protocolVersion);

            Biomes = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.VarInt);


            ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
            BlockEntities = reader.ReadArray(LengthFormat.VarInt,
                (ref MinecraftPrimitiveReader r1) => r1.ReadNbtTag(true));
        }
    }

    [PacketSubInfo(757, 762)]
    public sealed partial class V757_762 : MapChunkPacket
    {
        public NbtTag Heightmaps { get; set; }
        public ChunkBlockEntity[] BlockEntities { get; set; }

        public bool TrustEdges { get; set; }
        public long[] SkyLightMask { get; set; }
        public long[] BlockLightMask { get; set; }
        public long[] EmptySkyLightMask { get; set; }
        public long[] EmptyBlockLightMask { get; set; }

        public byte[][] SkyLight { get; set; }
        public byte[][] BlockLight { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            X = reader.ReadSignedInt();
            Z = reader.ReadSignedInt();
            Heightmaps = reader.ReadNbtTag(protocolVersion);
            ChunkData = reader.ReadBuffer(LengthFormat.VarInt);

            BlockEntities = reader.ReadArray(LengthFormat.VarInt,
                (ref MinecraftPrimitiveReader r1) => r1.ReadChunkBlockEntity(protocolVersion));

            TrustEdges = reader.ReadBoolean();
            SkyLightMask = reader.ReadArrayInt64BigEndian(reader.ReadVarInt());
            BlockLightMask = reader.ReadArrayInt64BigEndian(reader.ReadVarInt());
            EmptySkyLightMask = reader.ReadArrayInt64BigEndian(reader.ReadVarInt());
            EmptyBlockLightMask = reader.ReadArrayInt64BigEndian(reader.ReadVarInt());
            SkyLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader primitiveReader) =>
                primitiveReader.ReadArray(LengthFormat.VarInt, ReadDelegates.Byte)
            );
            BlockLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader primitiveReader) =>
                primitiveReader.ReadArray(LengthFormat.VarInt, ReadDelegates.Byte)
            );
        }
    }
    //public class V765 Ignore 

    [PacketSubInfo(763, 769)]
    public sealed partial class V763_769 : MapChunkPacket
    {
        public NbtTag Heightmaps { get; set; }
        public ChunkBlockEntity[] BlockEntities { get; set; }

        public long[] SkyLightMask { get; set; }
        public long[] BlockLightMask { get; set; }
        public long[] EmptySkyLightMask { get; set; }
        public long[] EmptyBlockLightMask { get; set; }

        public byte[][] SkyLight { get; set; }
        public byte[][] BlockLight { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            X = reader.ReadSignedInt();
            Z = reader.ReadSignedInt();

            Heightmaps = reader.ReadNbtTag(protocolVersion);


            ChunkData = reader.ReadBuffer(LengthFormat.VarInt);

            BlockEntities = reader.ReadArray(LengthFormat.VarInt,
                (ref MinecraftPrimitiveReader r1) => r1.ReadChunkBlockEntity(protocolVersion));

            try
            {
                SkyLightMask = reader.ReadArrayInt64BigEndian(reader.ReadVarInt());
                BlockLightMask = reader.ReadArrayInt64BigEndian(reader.ReadVarInt());
                EmptySkyLightMask = reader.ReadArrayInt64BigEndian(reader.ReadVarInt());
                EmptyBlockLightMask = reader.ReadArrayInt64BigEndian(reader.ReadVarInt());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            SkyLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader primitiveReader) =>
                primitiveReader.ReadBuffer(LengthFormat.VarInt)
            );
            BlockLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader primitiveReader) =>
                primitiveReader.ReadBuffer(LengthFormat.VarInt)
            );
        }
    }


    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
}