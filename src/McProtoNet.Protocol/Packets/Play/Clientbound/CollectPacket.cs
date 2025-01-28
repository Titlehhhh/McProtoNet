using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Collect", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class CollectPacket : IServerPacket
    {
        public int CollectedEntityId { get; set; }
        public int CollectorEntityId { get; set; }
        public int PickupItemCount { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : CollectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                CollectedEntityId = reader.ReadVarInt();
                CollectorEntityId = reader.ReadVarInt();
                PickupItemCount = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}