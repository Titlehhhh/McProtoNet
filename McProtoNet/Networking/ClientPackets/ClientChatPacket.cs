using McProtoNet.IO;
using McProtoNet.Networking;

public sealed class ClientChatPacket : Packet
{
    private String message { get; set; }

    public ClientChatPacket(String message)
    {
        this.message = message;
    }
    public override void Read(IMinecraftPrimitiveReader stream)
    {
        this.message = stream.ReadString();
    }

    public override void Write(IMinecraftPrimitiveWriter stream)
    {
        stream.WriteString(message);
    }
}
