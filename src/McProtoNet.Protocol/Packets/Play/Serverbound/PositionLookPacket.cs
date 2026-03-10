using System;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("PositionLook", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class PositionLookPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 767),
        new(768, MinecraftVersion.LatestProtocol)
    };

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }

    public VFirst_767Fields? VFirst_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                var fields = VFirst_767 ?? throw new InvalidOperationException("PositionLook VFirst_767 missing.");
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
                writer.WriteBoolean(fields.OnGround);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("PositionLook V768_Last missing.");
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
                writer.WriteMovementFlags(fields.Flags, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.PositionLook), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                VFirst_767 = new VFirst_767Fields
                {
                    OnGround = reader.ReadBoolean()
                };
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                V768_Last = new V768_LastFields
                {
                    Flags = reader.ReadMovementFlags(protocolVersion)
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.PositionLook), protocolVersion, SupportedVersionsStatic);
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
        public MovementFlags Flags { get; set; }
    }
}
