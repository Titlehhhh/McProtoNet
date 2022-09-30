using McProtoNet.NBT;
using McProtoNet.Protocol340.Data.World.Chunk;
using McProtoNet.Protocol340.Util;
using System;

namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerChunkDataPacket : Packet
    {
        public Column Column { get; private set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Column = stream.ReadChunkColumn();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }



        public ServerChunkDataPacket() { }
    }

}
