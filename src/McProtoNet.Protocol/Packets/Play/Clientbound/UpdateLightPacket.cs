using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("UpdateLight", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class UpdateLightPacket : IServerPacket
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }

        [PacketSubInfo(477, 710)]
        public sealed partial class V477_710 : UpdateLightPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
                SkyLightMask = reader.ReadVarInt();
                BlockLightMask = reader.ReadVarInt();
                EmptySkyLightMask = reader.ReadVarInt();
                EmptyBlockLightMask = reader.ReadVarInt();
                Data = reader.ReadRestBuffer();
            }

            public int SkyLightMask { get; set; }
            public int BlockLightMask { get; set; }
            public int EmptySkyLightMask { get; set; }
            public int EmptyBlockLightMask { get; set; }
            public byte[] Data { get; set; }
        }

        [PacketSubInfo(734, 754)]
        public sealed partial class V734_754 : UpdateLightPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
                TrustEdges = reader.ReadBoolean();
                SkyLightMask = reader.ReadVarInt();
                BlockLightMask = reader.ReadVarInt();
                EmptySkyLightMask = reader.ReadVarInt();
                EmptyBlockLightMask = reader.ReadVarInt();
                Data = reader.ReadRestBuffer();
            }

            public bool TrustEdges { get; set; }
            public int SkyLightMask { get; set; }
            public int BlockLightMask { get; set; }
            public int EmptySkyLightMask { get; set; }
            public int EmptyBlockLightMask { get; set; }
            public byte[] Data { get; set; }
        }

        [PacketSubInfo(755, 762)]
        public sealed partial class V755_762 : UpdateLightPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
                TrustEdges = reader.ReadBoolean();
                SkyLightMask = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.Int64);
                BlockLightMask = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.Int64);
                EmptySkyLightMask = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.Int64);
                EmptyBlockLightMask = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.Int64);
                SkyLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r_0) => r_0.ReadArray(LengthFormat.VarInt, ReadDelegates.Byte));
                BlockLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r_0) => r_0.ReadArray(LengthFormat.VarInt, ReadDelegates.Byte));
            }

            public bool TrustEdges { get; set; }
            public long[] SkyLightMask { get; set; }
            public long[] BlockLightMask { get; set; }
            public long[] EmptySkyLightMask { get; set; }
            public long[] EmptyBlockLightMask { get; set; }
            public byte[][] SkyLight { get; set; }
            public byte[][] BlockLight { get; set; }
        }

        [PacketSubInfo(763, 769)]
        public sealed partial class V763_769 : UpdateLightPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
                SkyLightMask = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.Int64);
                BlockLightMask = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.Int64);
                EmptySkyLightMask = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.Int64);
                EmptyBlockLightMask = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.Int64);
                SkyLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r_0) => r_0.ReadArray(LengthFormat.VarInt, ReadDelegates.Byte));
                BlockLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r_0) => r_0.ReadArray(LengthFormat.VarInt, ReadDelegates.Byte));
            }

            public long[] SkyLightMask { get; set; }
            public long[] BlockLightMask { get; set; }
            public long[] EmptySkyLightMask { get; set; }
            public long[] EmptyBlockLightMask { get; set; }
            public byte[][] SkyLight { get; set; }
            public byte[][] BlockLight { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}