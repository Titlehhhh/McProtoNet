using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Bed", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class BedPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public Position Location { get; set; }

        [PacketSubInfo(340, 404)]
        public sealed partial class V340_404 : BedPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}