using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("MultiBlockChange", PacketState.Play, PacketDirection.Clientbound)]
public abstract partial class MultiBlockChangePacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    [PacketSubInfo(340, 736)]
    public sealed partial class V340_736 : MultiBlockChangePacket
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
        public Record[] Records { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ChunkX = reader.ReadSignedInt();
            ChunkZ = reader.ReadSignedInt();
            int count = reader.ReadVarInt();
            Records = new Record[count];
            for (int i = 0; i < count; i++)
            {
                var horizontalPos = reader.ReadUnsignedByte();
                var y = reader.ReadUnsignedByte();
                var blockId = reader.ReadVarInt();
                Records[i] = new Record(horizontalPos, y, blockId);
            }
        }
        
        public readonly struct Record(byte horizontalPos, byte y, int blockId)
        {
            public readonly byte HorizontalPos = horizontalPos;
            public readonly byte Y = y;
            public readonly int BlockId = blockId;
        }
    }

    [PacketSubInfo(751, 756)]
    public sealed partial class V751_756 : MultiBlockChangePacket
    {
        public ChunkCoordinate ChunkCoordinates { get; set; }
        public long[]? Records { get; set; }
        public bool NotTrustEdges { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ChunkCoordinates = reader.ReadChunkCoordinate(protocolVersion);
            NotTrustEdges = reader.ReadBoolean();
            Records = reader.ReadArray<long, VarLongArrayReader>(LengthFormat.VarInt);
        }
    }


    [PacketSubInfo(757, 758)]
    public sealed partial class V757_758 : MultiBlockChangePacket

    {
        public ChunkCoordinate ChunkCoordinates { get; set; }
        public long[] Records { get; set; }

        public bool NotTrustEdges { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ChunkCoordinates = reader.ReadChunkCoordinate(protocolVersion);
            NotTrustEdges = reader.ReadBoolean();
            Records = reader.ReadArray<long, VarLongArrayReader>(LengthFormat.VarInt);
        }
    }

    [PacketSubInfo(759, 759)]
    public sealed partial class V759 : MultiBlockChangePacket
    {
        public ChunkCoordinate ChunkCoordinates { get; set; }
        public int[] Records { get; set; }

        public bool NotTrustEdges { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ChunkCoordinates = reader.ReadChunkCoordinate(protocolVersion);

            NotTrustEdges = reader.ReadBoolean();
            Records = reader.ReadArray<int, VarIntArrayReader>(LengthFormat.VarInt);
        }
    }

    [PacketSubInfo(760, 762)]
    public sealed partial class V760_762 : MultiBlockChangePacket
    {
        public ChunkCoordinate ChunkCoordinates { get; set; }
        public int[] Records { get; set; }

        public bool SuppressLightUpdates { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)

        {
            ChunkCoordinates = reader.ReadChunkCoordinate(protocolVersion);
            SuppressLightUpdates = reader.ReadBoolean();
            Records = reader.ReadArray<int, VarIntArrayReader>(LengthFormat.VarInt);
        }
    }


    [PacketSubInfo(763, 769)]
    public sealed partial class V763_769 : MultiBlockChangePacket
    {
        public ChunkCoordinate ChunkCoordinates { get; set; }
        public int[] Records { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ChunkCoordinates = reader.ReadChunkCoordinate(protocolVersion);
            Records = reader.ReadArray<int, VarIntArrayReader>(LengthFormat.VarInt);
        }
    }
}