using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SpawnEntityPainting", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SpawnEntityPaintingPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public Guid EntityUUID { get; set; }
        public Position Location { get; set; }
        public byte Direction { get; set; }

        [PacketSubInfo(340, 351)]
        public sealed partial class V340_351 : SpawnEntityPaintingPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EntityUUID = reader.ReadUUID();
                Title = reader.ReadString();
                Location = reader.ReadPosition(protocolVersion);
                Direction = reader.ReadUnsignedByte();
            }

            public string Title { get; set; }
        }

        [PacketSubInfo(393, 758)]
        public sealed partial class V393_758 : SpawnEntityPaintingPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EntityUUID = reader.ReadUUID();
                Title = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
                Direction = reader.ReadUnsignedByte();
            }

            public int Title { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}