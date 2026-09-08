using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class FacePlayerEntityTarget : IProtocolType<FacePlayerEntityTarget>
{
    public int EntityId { get; }
    public string FeetEyesName { get; }
    public int FeetEyes { get; }

    public FacePlayerEntityTarget(int entityId, string feetEyesName, int feetEyes)
    {
        EntityId = entityId;
        FeetEyesName = feetEyesName;
        FeetEyes = feetEyes;
    }

    public static FacePlayerEntityTarget Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FacePlayerEntityTarget>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var entityId = reader.ReadVarInt();
            var feetEyesName = reader.ReadString();
            return new FacePlayerEntityTarget(entityId, feetEyesName, default!);
        }

        if (protocolVersion >= 765)
        {
            var entityId = reader.ReadVarInt();
            var feetEyes = reader.ReadVarInt();
            return new FacePlayerEntityTarget(entityId, default!, feetEyes);
        }

        throw new System.NotSupportedException($"FacePlayerEntityTarget has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FacePlayerEntityTarget>(protocolVersion);
        if (protocolVersion <= 764)
        {
            writer.WriteVarInt(EntityId);
            writer.WriteString(FeetEyesName);
            return;
        }

        if (protocolVersion >= 765)
        {
            writer.WriteVarInt(EntityId);
            writer.WriteVarInt(FeetEyes);
            return;
        }

        throw new System.NotSupportedException($"FacePlayerEntityTarget has no wire layout for protocol version {protocolVersion}.");
    }
}
