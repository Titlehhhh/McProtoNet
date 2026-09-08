using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(760, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.hide_message", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("MessageSignature", "byte[]", Group = "V760", From = 760, To = 760)]
[PacketField("Id", "int", Group = "V761_Last", From = 761)]
[PacketField("Signature", "byte[]?", Group = "V761_Last", From = 761)]
public sealed partial record HideMessagePacket(HideMessagePacket.V760Layer? V760 = null, HideMessagePacket.V761_LastLayer? V761_Last = null) : IPacket<HideMessagePacket>, IPacket
{
    public readonly record struct V760Layer(byte[] MessageSignature);
    public readonly record struct V761_LastLayer(int Id, byte[]? Signature);
    public static HideMessagePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HideMessagePacket>(protocolVersion);
        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var messageSignature = reader.ReadByteArray();
            return new HideMessagePacket(V760: new V760Layer(messageSignature));
        }

        if (protocolVersion >= 761)
        {
            var id = reader.ReadVarInt();
            byte[]? signature = default;
            if (id == 0)
            {
                var signatureValue = reader.ReadFixedBytes(256);
                signature = signatureValue;
            }

            return new HideMessagePacket(V761_Last: new V761_LastLayer(id, signature));
        }

        throw new System.NotSupportedException($"HideMessagePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HideMessagePacket>(protocolVersion);
        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var layer = V760 ?? throw new WrongLayerException("HideMessagePacket", protocolVersion, "V760");
            byte[] MessageSignature = layer.MessageSignature;
            writer.WriteByteArray(MessageSignature);
            return;
        }

        if (protocolVersion >= 761)
        {
            var layer = V761_Last ?? throw new WrongLayerException("HideMessagePacket", protocolVersion, "V761_Last");
            int Id = layer.Id;
            byte[]? Signature = layer.Signature;
            writer.WriteVarInt(Id);
            if (Id == 0)
            {
                writer.WriteFixedBytes((Signature ?? throw new System.InvalidOperationException("Signature is required at this protocol version.")), 256);
            }
            else if (Signature is not null)
            {
                throw new System.InvalidOperationException("Signature is set, but 'id' does not select it at this protocol version.");
            }

            return;
        }

        throw new System.NotSupportedException($"HideMessagePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.hide_message", "HideMessage", PacketPhase.Play, PacketDirection.Clientbound, 51);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x16;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 776)
        {
            id = 0x1F;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
