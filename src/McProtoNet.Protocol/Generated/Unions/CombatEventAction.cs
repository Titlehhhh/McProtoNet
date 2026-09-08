using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[Union]
public partial record CombatEventAction
{
    partial record Enter();
    partial record End(int Duration, int EntityId);
    partial record Death(int PlayerId, int EntityId, string MessageJson);
    public static CombatEventAction Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CombatEventAction>(protocolVersion);
        switch (discriminator)
        {
            case 0:
            {
                return new Enter();
            }

            case 1:
            {
                var duration = reader.ReadVarInt();
                var entityId = reader.ReadSignedInt();
                return new End(duration, entityId);
            }

            case 2:
            {
                var playerId = reader.ReadVarInt();
                var entityId = reader.ReadSignedInt();
                var messageJson = reader.ReadString();
                return new Death(playerId, entityId, messageJson);
            }
        }

        throw new System.NotSupportedException($"CombatEventAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CombatEventAction>(protocolVersion);
        switch (this)
        {
            case Enter _:
            {
                return;
            }

            case End arm:
            {
                int Duration = arm.Duration;
                int EntityId = arm.EntityId;
                writer.WriteVarInt(Duration);
                writer.WriteSignedInt(EntityId);
                return;
            }

            case Death arm:
            {
                int PlayerId = arm.PlayerId;
                int EntityId = arm.EntityId;
                string MessageJson = arm.MessageJson;
                writer.WriteVarInt(PlayerId);
                writer.WriteSignedInt(EntityId);
                writer.WriteString(MessageJson);
                return;
            }
        }

        throw new System.NotSupportedException($"CombatEventAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        switch (this)
        {
            case Enter _:
                return 0;
            case End _:
                return 1;
            case Death _:
                return 2;
        }

        throw new System.NotSupportedException($"CombatEventAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }
}
