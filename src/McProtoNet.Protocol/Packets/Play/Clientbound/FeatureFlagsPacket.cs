using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("FeatureFlags", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class FeatureFlagsPacket : IServerPacket
    {
        public string[] Features { get; set; }

        [PacketSubInfo(761, 763)]
        public sealed partial class V761_763 : FeatureFlagsPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Features = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.String);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}