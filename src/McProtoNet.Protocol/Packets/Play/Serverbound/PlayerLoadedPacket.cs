using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("PlayerLoaded", PacketState.Play, PacketDirection.Serverbound)]
    public partial class PlayerLoadedPacket : IClientPacket
    {
        [PacketSubInfo(769, 769)]
        public sealed partial class V769 : PlayerLoadedPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V769.IsSupportedVersionStatic(protocolVersion))
                V769.SerializeInternal(ref writer, protocolVersion);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.PlayerLoaded), protocolVersion);
        }
    }
}