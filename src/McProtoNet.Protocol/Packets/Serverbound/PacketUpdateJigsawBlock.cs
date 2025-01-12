using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class UpdateJigsawBlockPacket : IClientPacket
    {
        public Position Location { get; set; }
        public string FinalState { get; set; }

        public sealed class V477_578 : UpdateJigsawBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, AttachmentType, TargetPool, FinalState);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, string attachmentType, string targetPool, string finalState)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteString(attachmentType);
                writer.WriteString(targetPool);
                writer.WriteString(finalState);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 477 and <= 578;
            }

            public string AttachmentType { get; set; }
            public string TargetPool { get; set; }
        }

        public sealed class V709_764 : UpdateJigsawBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Name, Target, Pool, FinalState, JointType);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, string name, string target, string pool, string finalState, string jointType)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteString(name);
                writer.WriteString(target);
                writer.WriteString(pool);
                writer.WriteString(finalState);
                writer.WriteString(jointType);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 709 and <= 764;
            }

            public string Name { get; set; }
            public string Target { get; set; }
            public string Pool { get; set; }
            public string JointType { get; set; }
        }

        public sealed class V765_769 : UpdateJigsawBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Name, Target, Pool, FinalState, JointType, SelectionPriority, PlacementPriority);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, string name, string target, string pool, string finalState, string jointType, int selectionPriority, int placementPriority)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteString(name);
                writer.WriteString(target);
                writer.WriteString(pool);
                writer.WriteString(finalState);
                writer.WriteString(jointType);
                writer.WriteVarInt(selectionPriority);
                writer.WriteVarInt(placementPriority);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 765 and <= 769;
            }

            public string Name { get; set; }
            public string Target { get; set; }
            public string Pool { get; set; }
            public string JointType { get; set; }
            public int SelectionPriority { get; set; }
            public int PlacementPriority { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V477_578.SupportedVersion(protocolVersion) || V709_764.SupportedVersion(protocolVersion) || V765_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V477_578.SupportedVersion(protocolVersion))
                V477_578.SerializeInternal(ref writer, protocolVersion, Location, string.Empty, string.Empty, FinalState);
            else if (V709_764.SupportedVersion(protocolVersion))
                V709_764.SerializeInternal(ref writer, protocolVersion, Location, string.Empty, string.Empty, string.Empty, FinalState, string.Empty);
            else if (V765_769.SupportedVersion(protocolVersion))
                V765_769.SerializeInternal(ref writer, protocolVersion, Location, string.Empty, string.Empty, string.Empty, FinalState, string.Empty, 0, 0);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.UpdateJigsawBlock), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.UpdateJigsawBlock;

        public ClientPacket GetPacketId() => PacketId;
    }
}