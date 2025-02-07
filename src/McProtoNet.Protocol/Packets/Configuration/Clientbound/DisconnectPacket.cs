using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("Disconnect", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class DisconnectPacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    [PacketSubInfo(764, 764)]
    public sealed partial class V764 : DisconnectPacket
    {
        public string Reason { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Reason = reader.ReadString();
        }
    }

    [PacketSubInfo(765, 769)]
    public sealed partial class V765_769 : DisconnectPacket
    {
        public NbtTag Reason { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Reason = reader.ReadNbtTag(readRootTag: false);
        }
    }
}