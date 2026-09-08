using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Union]
public partial record WaypointIdentity
{
    partial record Uuid(Guid Value);
    partial record Id(string Value);
    public static WaypointIdentity Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WaypointIdentity>(protocolVersion);
        switch (discriminator)
        {
            case 1:
            {
                var value = reader.ReadUUID();
                return new Uuid(value);
            }

            case 0:
            {
                var value = reader.ReadString();
                return new Id(value);
            }
        }

        throw new System.NotSupportedException($"WaypointIdentity has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WaypointIdentity>(protocolVersion);
        switch (this)
        {
            case Uuid arm:
            {
                Guid Value = arm.Value;
                writer.WriteUUID(Value);
                return;
            }

            case Id arm:
            {
                string Value = arm.Value;
                writer.WriteString(Value);
                return;
            }
        }

        throw new System.NotSupportedException($"WaypointIdentity case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        switch (this)
        {
            case Uuid _:
                return 1;
            case Id _:
                return 0;
        }

        throw new System.NotSupportedException($"WaypointIdentity case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }
}
