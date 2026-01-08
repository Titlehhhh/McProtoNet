﻿using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("UpdateTime", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class UpdateTimePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 767),
        new(768, MinecraftVersion.LatestProtocol),
    };

    public long Age { get; set; }
    public long Time { get; set; }

    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
                writer.WriteSignedLong(Age);
                writer.WriteSignedLong(Time);
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("UpdateTime V768_Last missing.");
                writer.WriteSignedLong(Age);
                writer.WriteSignedLong(Time);
                writer.WriteBoolean(fields.TickDayTime);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.UpdateTime), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
                Age = reader.ReadSignedLong();
                Time = reader.ReadSignedLong();
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V768_LastFields();
                Age = reader.ReadSignedLong();
                Time = reader.ReadSignedLong();
                fields.TickDayTime = reader.ReadBoolean();
                V768_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.UpdateTime), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V768_LastFields
    {
        public bool TickDayTime { get; set; }
    }
}
