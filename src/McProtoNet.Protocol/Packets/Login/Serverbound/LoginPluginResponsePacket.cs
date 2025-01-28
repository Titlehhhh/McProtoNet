using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginPluginResponse", PacketState.Login, PacketDirection.Serverbound)]
public partial class LoginPluginResponsePacket : IClientPacket
{
    public int MessageId { get; set; }
    public byte[]? Data { get; set; }

    [PacketSubInfo(393, 769)]
    public sealed partial class V393_769 : LoginPluginResponsePacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, MessageId, Data);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            int messageId, byte[]? data)
        {
            writer.WriteVarInt(messageId);
            writer.WriteBoolean(data is not null);
            if (data is not null)
            {
                writer.WriteBuffer(data!);
            }
        }
    }


    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V393_769.IsSupportedVersionStatic(protocolVersion))
        {
            V393_769.SerializeInternal(ref writer, protocolVersion, MessageId, Data);
        }
        else
        {
            throw new ProtocolNotSupportException(ClientLoginPacket.LoginPluginResponse.ToString(), protocolVersion);
        }
    }
}