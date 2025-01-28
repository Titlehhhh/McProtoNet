using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("KeepAlive", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class KeepAlivePacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

  
    
    [PacketSubInfo(764,769)]
    public sealed partial class V764_769 : KeepAlivePacket
    {
        public long KeepAliveId { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            KeepAliveId = reader.ReadSignedLong();
        }

    }

}