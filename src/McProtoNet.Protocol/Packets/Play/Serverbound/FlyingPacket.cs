using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("Flying", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class FlyingPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 767),
        new(768, MinecraftVersion.LatestProtocol)
    };

    public VFirst_767Fields? VFirst_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                var fields = VFirst_767 ?? throw new InvalidOperationException("Flying VFirst_767 fields missing.");
                writer.WriteBoolean(fields.OnGround);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("Flying V768_Last fields missing.");
                writer.WriteUnsignedByte(fields.Flags);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.Flying), protocolVersion, SupportedVersionsStatic);
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
                    OnGround = reader.ReadBoolean()
                };
                V768_Last = null;
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                V768_Last = new V768_LastFields
                {
                    Flags = reader.ReadUnsignedByte()
                };
                VFirst_767 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.Flying), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_767Fields
    {
        public bool OnGround { get; set; }
    }

    public struct V768_LastFields
    {
        public byte Flags { get; set; }
    }
}
