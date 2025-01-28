using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("CustomPayload", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class CustomPayloadPacket : IServerPacket
{
    [PacketSubInfo(764,769)]
    public sealed partial class V764_769 : CustomPayloadPacket
    {
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Channel = reader.ReadString();
            Data = reader.ReadRestBuffer();
        }

    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

}