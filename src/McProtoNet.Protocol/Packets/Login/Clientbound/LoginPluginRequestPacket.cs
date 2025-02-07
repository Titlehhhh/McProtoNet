using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("LoginPluginRequest", PacketState.Login, PacketDirection.Clientbound)]
public abstract partial class LoginPluginRequestPacket : IServerPacket
{
    public int MessageId { get; set; }
    public string Channel { get; set; }
    public byte[] Data { get; set; }


    [PacketSubInfo(393, 769)]
    public sealed partial class V393_769 : LoginPluginRequestPacket
    {
        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            MessageId = reader.ReadVarInt();
            Channel = reader.ReadString();
            Data = reader.ReadRestBuffer();
        }
    }


    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
}