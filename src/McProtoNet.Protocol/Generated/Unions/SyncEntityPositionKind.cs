using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(777, MinecraftVersion.LatestProtocol)]
[Union]
public partial record SyncEntityPositionKind
{
    partial record Linear(double X, double Y, double Z);
    partial record Stepped(SyncEntityStep[] Steps);
    public static SyncEntityPositionKind Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityPositionKind>(protocolVersion);
        switch (discriminator)
        {
            case 0:
            {
                var x = reader.ReadDouble();
                var y = reader.ReadDouble();
                var z = reader.ReadDouble();
                return new Linear(x, y, z);
            }

            case 1:
            {
                int stepsCount = reader.ReadVarInt();
                var steps = new SyncEntityStep[stepsCount];
                for (int i = 0; i < steps.Length; i++)
                    steps[i] = reader.ReadType<SyncEntityStep>(protocolVersion);
                return new Stepped(steps);
            }
        }

        throw new System.NotSupportedException($"SyncEntityPositionKind has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityPositionKind>(protocolVersion);
        switch (this)
        {
            case Linear arm:
            {
                double X = arm.X;
                double Y = arm.Y;
                double Z = arm.Z;
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                return;
            }

            case Stepped arm:
            {
                SyncEntityStep[] Steps = arm.Steps;
                writer.WriteVarInt(Steps.Length);
                foreach (var stepsItem in Steps)
                    writer.WriteType<SyncEntityStep>(stepsItem, protocolVersion);
                return;
            }
        }

        throw new System.NotSupportedException($"SyncEntityPositionKind case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        switch (this)
        {
            case Linear _:
                return 0;
            case Stepped _:
                return 1;
        }

        throw new System.NotSupportedException($"SyncEntityPositionKind case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }
}
