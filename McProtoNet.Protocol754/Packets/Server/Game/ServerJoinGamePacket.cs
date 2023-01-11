using McProtoNet.NBT;
using McProtoNet.Protocol754.Data;

namespace McProtoNet.Protocol754.Packets.Server
{


    public sealed class ServerJoinGamePacket : MinecraftPacket
    {
        private static byte GAMEMODE_MASK = 0x07;

        public int EntityId { get; set; }
        public bool Hardcore { get; set; }
        public GameMode GameMode { get; set; }
        public GameMode PreviousGamemode { get; set; }
        public int WorldCount { get; set; }
        public string[] WorldNames { get; set; }
        public NbtCompound DimensionCodec { get; set; }
        public NbtCompound Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public int MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool Debug { get; set; }
        public bool Flat { get; set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteInt(this.EntityId);
            stream.WriteBoolean(this.Hardcore);

            stream.WriteUnsignedByte((byte)this.GameMode);
            stream.WriteByte((sbyte)PreviousGamemode);
            stream.WriteVarInt(this.WorldCount);
            foreach (string worldName in this.WorldNames)
                stream.WriteString(worldName);
            //NBT.Write(stream, this.dimensionCodec);
            stream.WriteNbt(this.DimensionCodec);
            //NBT.Write(stream, this.dimension);
            stream.WriteNbt(this.Dimension);
            //    stream.WriteString(this.worldName);
            stream.WriteString(this.WorldName);
            //    stream.WriteLong(this.hashedSeed);
            stream.WriteLong(this.HashedSeed);
            //    stream.WriteVarInt(this.maxPlayers);
            stream.WriteVarInt(MaxPlayers);
            //    stream.WriteVarInt(this.viewDistance);
            stream.WriteVarInt(ViewDistance);
            //    stream.WriteBoolean(this.reducedDebugInfo);
            stream.WriteBoolean(ReducedDebugInfo);
            //    stream.WriteBoolean(this.enableRespawnScreen);
            stream.WriteBoolean(EnableRespawnScreen);
            //    stream.WriteBoolean(this.debug);
            stream.WriteBoolean(Debug);
            //    stream.WriteBoolean(this.flat);
            stream.WriteBoolean(Flat);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerJoinGamePacket() { }
    }
}

