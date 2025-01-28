using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("RemoveResourcePack", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class RemoveResourcePackPacket : IServerPacket
    {
        public Guid? Uuid { get; set; }

        [PacketSubInfo(765, 767)]
        public sealed partial class V765_767 : RemoveResourcePackPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Uuid = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadUUID());
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}