using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("FeatureFlags", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class FeatureFlagsPacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    [PacketSubInfo(764,769)]
    public sealed partial class V764_769 : FeatureFlagsPacket
    {
        public string[] Features { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            var count = reader.ReadVarInt();
            Features = new string[count];
            for (var i = 0; i < count; i++)
            {
                Features[i] = reader.ReadString();
            }
        }

    }

}