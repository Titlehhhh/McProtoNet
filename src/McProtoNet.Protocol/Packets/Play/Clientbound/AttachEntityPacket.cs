using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("AttachEntity", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class AttachEntityPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public int VehicleId { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : AttachEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                VehicleId = reader.ReadSignedInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}