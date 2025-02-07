using McProtoNet.Serialization;
using McProtoNet.Protocol;

namespace McProtoNet.Protocol.Packets.Login.Clientbound
{
    [PacketInfo("Compress", PacketState.Login, PacketDirection.Clientbound)]
    public abstract partial class CompressPacket : IServerPacket
    {
        public int Threshold { get; set; }

        [PacketSubInfo(340, 769)]
        public sealed partial class V340_769 : CompressPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Threshold = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}