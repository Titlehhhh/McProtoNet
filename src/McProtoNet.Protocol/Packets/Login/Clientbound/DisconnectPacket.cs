using McProtoNet.Protocol;
using McProtoNet.Serialization;


namespace McProtoNet.Protocol.Packets.Login.Clientbound;
    
[PacketInfo("Disconnect", PacketState.Login, PacketDirection.Clientbound)]
public abstract partial class DisconnectPacket : IServerPacket
{
    public string Reason { get; set; }

    [PacketSubInfo(340,769)]
    internal sealed partial class V340_769 : DisconnectPacket
    {

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Reason = reader.ReadString();
        }
    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

}