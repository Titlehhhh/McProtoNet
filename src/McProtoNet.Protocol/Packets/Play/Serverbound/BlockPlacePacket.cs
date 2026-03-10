using System;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("BlockPlace", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class BlockPlacePacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, 767),
        new(768, MinecraftVersion.LatestProtocol)
    };

    public Position Location { get; set; }
    public int Direction { get; set; }
    public int Hand { get; set; }
    public float CursorX { get; set; }
    public float CursorY { get; set; }
    public float CursorZ { get; set; }

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759_767Fields? V759_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("BlockPlace VFirst_758 fields missing.");
                writer.WriteVarInt(Hand);
                writer.WritePosition(Location, protocolVersion);
                writer.WriteVarInt(Direction);
                writer.WriteFloat(CursorX);
                writer.WriteFloat(CursorY);
                writer.WriteFloat(CursorZ);
                writer.WriteBoolean(fields.InsideBlock);
                return;
            }
            case >= 759 and <= 767:
            {
                var fields = V759_767 ?? throw new InvalidOperationException("BlockPlace V759_767 fields missing.");
                writer.WriteVarInt(Hand);
                writer.WritePosition(Location, protocolVersion);
                writer.WriteVarInt(Direction);
                writer.WriteFloat(CursorX);
                writer.WriteFloat(CursorY);
                writer.WriteFloat(CursorZ);
                writer.WriteBoolean(fields.InsideBlock);
                writer.WriteVarInt(fields.Sequence);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("BlockPlace V768_Last fields missing.");
                writer.WriteVarInt(Hand);
                writer.WritePosition(Location, protocolVersion);
                writer.WriteVarInt(Direction);
                writer.WriteFloat(CursorX);
                writer.WriteFloat(CursorY);
                writer.WriteFloat(CursorZ);
                writer.WriteBoolean(fields.InsideBlock);
                writer.WriteBoolean(fields.WorldBorderHit);
                writer.WriteVarInt(fields.Sequence);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.BlockPlace), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                Hand = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
                Direction = reader.ReadVarInt();
                CursorX = reader.ReadFloat();
                CursorY = reader.ReadFloat();
                CursorZ = reader.ReadFloat();
                VFirst_758 = new VFirst_758Fields
                {
                    InsideBlock = reader.ReadBoolean()
                };
                V759_767 = null;
                V768_Last = null;
                return;
            case >= 759 and <= 767:
                Hand = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
                Direction = reader.ReadVarInt();
                CursorX = reader.ReadFloat();
                CursorY = reader.ReadFloat();
                CursorZ = reader.ReadFloat();
                V759_767 = new V759_767Fields
                {
                    InsideBlock = reader.ReadBoolean(),
                    Sequence = reader.ReadVarInt()
                };
                VFirst_758 = null;
                V768_Last = null;
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                Hand = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
                Direction = reader.ReadVarInt();
                CursorX = reader.ReadFloat();
                CursorY = reader.ReadFloat();
                CursorZ = reader.ReadFloat();
                V768_Last = new V768_LastFields
                {
                    InsideBlock = reader.ReadBoolean(),
                    WorldBorderHit = reader.ReadBoolean(),
                    Sequence = reader.ReadVarInt()
                };
                VFirst_758 = null;
                V759_767 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.BlockPlace), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_758Fields
    {
        public bool InsideBlock { get; set; }
    }

    public struct V759_767Fields
    {
        public bool InsideBlock { get; set; }
        public int Sequence { get; set; }
    }

    public struct V768_LastFields
    {
        public bool InsideBlock { get; set; }
        public bool WorldBorderHit { get; set; }
        public int Sequence { get; set; }
    }
}
