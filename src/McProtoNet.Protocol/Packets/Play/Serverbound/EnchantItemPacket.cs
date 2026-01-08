using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("EnchantItem", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class EnchantItemPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 767),
        new(768, MinecraftVersion.LatestProtocol)
    };

    public sbyte Enchantment { get; set; }

    public VFirst_767Fields? VFirst_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                var fields = VFirst_767 ?? throw new InvalidOperationException("EnchantItem VFirst_767 fields missing.");
                writer.WriteSignedByte(fields.WindowId);
                writer.WriteSignedByte(Enchantment);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("EnchantItem V768_Last fields missing.");
                writer.WriteVarInt(fields.WindowId);
                writer.WriteSignedByte(Enchantment);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.EnchantItem), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
                VFirst_767 = new VFirst_767Fields
                {
                    WindowId = reader.ReadSignedByte()
                };
                Enchantment = reader.ReadSignedByte();
                V768_Last = null;
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                V768_Last = new V768_LastFields
                {
                    WindowId = reader.ReadVarInt()
                };
                Enchantment = reader.ReadSignedByte();
                VFirst_767 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.EnchantItem), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_767Fields
    {
        public sbyte WindowId { get; set; }
    }

    public struct V768_LastFields
    {
        public int WindowId { get; set; }
    }
}
