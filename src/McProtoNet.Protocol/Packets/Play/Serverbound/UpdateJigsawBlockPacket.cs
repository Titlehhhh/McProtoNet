using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("UpdateJigsawBlock", PacketState.Play, PacketDirection.Serverbound)]
    public partial class UpdateJigsawBlockPacket : IClientPacket
    {
        public Position Location { get; set; }
        public string FinalState { get; set; }

        [PacketSubInfo(477, 578)]
        public sealed partial class V477_578 : UpdateJigsawBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, AttachmentType, TargetPool, FinalState);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Position location, string attachmentType, string targetPool, string finalState)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteString(attachmentType);
                writer.WriteString(targetPool);
                writer.WriteString(finalState);
            }

            public string AttachmentType { get; set; }
            public string TargetPool { get; set; }
        }

        [PacketSubInfo(709, 764)]
        public sealed partial class V709_764 : UpdateJigsawBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Name, Target, Pool, FinalState, JointType);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Position location, string name, string target, string pool, string finalState, string jointType)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteString(name);
                writer.WriteString(target);
                writer.WriteString(pool);
                writer.WriteString(finalState);
                writer.WriteString(jointType);
            }

            public string Name { get; set; }
            public string Target { get; set; }
            public string Pool { get; set; }
            public string JointType { get; set; }
        }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : UpdateJigsawBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Name, Target, Pool, FinalState, JointType,
                    SelectionPriority, PlacementPriority);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Position location, string name, string target, string pool, string finalState, string jointType,
                int selectionPriority, int placementPriority)
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

            public string Name { get; set; }
            public string Target { get; set; }
            public string Pool { get; set; }
            public string JointType { get; set; }
            public int SelectionPriority { get; set; }
            public int PlacementPriority { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V477_578.IsSupportedVersionStatic(protocolVersion))
                V477_578.SerializeInternal(ref writer, protocolVersion, Location, string.Empty, string.Empty,
                    FinalState);
            else if (V709_764.IsSupportedVersionStatic(protocolVersion))
                V709_764.SerializeInternal(ref writer, protocolVersion, Location, string.Empty, string.Empty,
                    string.Empty, FinalState, string.Empty);
            else if (V765_769.IsSupportedVersionStatic(protocolVersion))
                V765_769.SerializeInternal(ref writer, protocolVersion, Location, string.Empty, string.Empty,
                    string.Empty, FinalState, string.Empty, 0, 0);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.UpdateJigsawBlock), protocolVersion);
        }
    }
}