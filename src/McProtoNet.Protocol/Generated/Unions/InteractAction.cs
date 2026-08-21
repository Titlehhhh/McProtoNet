using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, 774)]
[Union]
public partial record InteractAction
{
    partial record Interact(int Hand);
    partial record Attack();
    partial record InteractAt(float X, float Y, float Z, int Hand);
    public static InteractAction Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<InteractAction>(protocolVersion);
        switch (discriminator)
        {
            case 0:
            {
                var hand = reader.ReadVarInt();
                return new Interact(hand);
            }

            case 1:
            {
                return new Attack();
            }

            case 2:
            {
                var x = reader.ReadFloat();
                var y = reader.ReadFloat();
                var z = reader.ReadFloat();
                var hand = reader.ReadVarInt();
                return new InteractAt(x, y, z, hand);
            }
        }

        throw new System.NotSupportedException($"InteractAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<InteractAction>(protocolVersion);
        switch (this)
        {
            case Interact arm:
            {
                int Hand = arm.Hand;
                writer.WriteVarInt(Hand);
                return;
            }

            case Attack _:
            {
                return;
            }

            case InteractAt arm:
            {
                float X = arm.X;
                float Y = arm.Y;
                float Z = arm.Z;
                int Hand = arm.Hand;
                writer.WriteFloat(X);
                writer.WriteFloat(Y);
                writer.WriteFloat(Z);
                writer.WriteVarInt(Hand);
                return;
            }
        }

        throw new System.NotSupportedException($"InteractAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        switch (this)
        {
            case Interact _:
                return 0;
            case Attack _:
                return 1;
            case InteractAt _:
                return 2;
        }

        throw new System.NotSupportedException($"InteractAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }
}
