using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ResourcePackSend", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class ResourcePackSendPacket : IServerPacket
{
    [PacketSubInfo(764, 764)]
    public sealed partial class V764 : ResourcePackSendPacket
    {
        public string Url { get; set; }
        public string Hash { get; set; }
        public bool Forced { get; set; }
        public string? PromptMessage { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Url = reader.ReadString();
            Hash = reader.ReadString();
            Forced = reader.ReadBoolean();
            PromptMessage = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadString());
        }
    }


    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
}