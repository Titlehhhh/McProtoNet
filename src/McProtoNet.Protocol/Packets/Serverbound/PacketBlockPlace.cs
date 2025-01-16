using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class BlockPlacePacket : IClientPacket
    {
        public Position Location { get; set; }
        public int Direction { get; set; }
        public int Hand { get; set; }
        public float CursorX { get; set; }
        public float CursorY { get; set; }
        public float CursorZ { get; set; }

        public sealed class V340_404 : BlockPlacePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Direction, Hand, CursorX, CursorY, CursorZ);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, int direction, int hand, float cursorX, float cursorY, float cursorZ)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(direction);
                writer.WriteVarInt(hand);
                writer.WriteFloat(cursorX);
                writer.WriteFloat(cursorY);
                writer.WriteFloat(cursorZ);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 404;
            }
        }

        public sealed class V477_758 : BlockPlacePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY, CursorZ, InsideBlock);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand, Position location, int direction, float cursorX, float cursorY, float cursorZ, bool insideBlock)
            {
                writer.WriteVarInt(hand);
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(direction);
                writer.WriteFloat(cursorX);
                writer.WriteFloat(cursorY);
                writer.WriteFloat(cursorZ);
                writer.WriteBoolean(insideBlock);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 477 and <= 758;
            }

            public bool InsideBlock { get; set; }
        }

        public sealed class V759_767 : BlockPlacePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY, CursorZ, InsideBlock, Sequence);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand, Position location, int direction, float cursorX, float cursorY, float cursorZ, bool insideBlock, int sequence)
            {
                writer.WriteVarInt(hand);
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(direction);
                writer.WriteFloat(cursorX);
                writer.WriteFloat(cursorY);
                writer.WriteFloat(cursorZ);
                writer.WriteBoolean(insideBlock);
                writer.WriteVarInt(sequence);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 767;
            }

            public bool InsideBlock { get; set; }
            public int Sequence { get; set; }
        }

        public sealed class V768_769 : BlockPlacePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY, CursorZ, InsideBlock, WorldBorderHit, Sequence);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand, Position location, int direction, float cursorX, float cursorY, float cursorZ, bool insideBlock, bool worldBorderHit, int sequence)
            {
                writer.WriteVarInt(hand);
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(direction);
                writer.WriteFloat(cursorX);
                writer.WriteFloat(cursorY);
                writer.WriteFloat(cursorZ);
                writer.WriteBoolean(insideBlock);
                writer.WriteBoolean(worldBorderHit);
                writer.WriteVarInt(sequence);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }

            public bool InsideBlock { get; set; }
            public bool WorldBorderHit { get; set; }
            public int Sequence { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_404.SupportedVersion(protocolVersion) || V477_758.SupportedVersion(protocolVersion) || V759_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_404.SupportedVersion(protocolVersion))
                V340_404.SerializeInternal(ref writer, protocolVersion, Location, Direction, Hand, CursorX, CursorY, CursorZ);
            else if (V477_758.SupportedVersion(protocolVersion))
                V477_758.SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY, CursorZ, false);
            else if (V759_767.SupportedVersion(protocolVersion))
                V759_767.SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY, CursorZ, false, 0);
            else if (V768_769.SupportedVersion(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY, CursorZ, false, false, 0);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.BlockPlace), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.BlockPlace;

        public ClientPacket GetPacketId() => PacketId;
    }
}