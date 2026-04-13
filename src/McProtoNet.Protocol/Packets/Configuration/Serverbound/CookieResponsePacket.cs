using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[PacketInfo("CookieResponse", PacketState.Configuration, PacketDirection.Serverbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x01)]
public sealed partial class CookieResponsePacket : IPacket
{
    public string Key { get; set; } = "";
    public byte[]? Value { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteString(Key);
                if (Value is null)
                {
                    writer.WriteVarInt(-1);
                }
                else
                {
                    writer.WriteVarInt(Value.Length);
                    writer.WriteBuffer(Value);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CookieResponsePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                Key = reader.ReadString();
                int length = reader.ReadVarInt();
                if (length < 0)
                {
                    Value = null;
                }
                else
                {
                    Value = reader.ReadBuffer(length);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CookieResponsePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}