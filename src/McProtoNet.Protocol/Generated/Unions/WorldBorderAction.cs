using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[Union]
public partial record WorldBorderAction
{
    partial record SetSize(double Diameter);
    partial record LerpSize(double OldDiameter, double NewDiameter, long Speed);
    partial record SetCenter(double X, double Z);
    partial record Initialize(double X, double Z, double OldDiameter, double NewDiameter, long Speed, int PortalTeleportBoundary, int WarningTime, int WarningBlocks);
    partial record SetWarningTime(int WarningTime);
    partial record SetWarningBlocks(int WarningBlocks);
    public static WorldBorderAction Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderAction>(protocolVersion);
        switch (discriminator)
        {
            case 0:
            {
                var diameter = reader.ReadDouble();
                return new SetSize(diameter);
            }

            case 1:
            {
                var oldDiameter = reader.ReadDouble();
                var newDiameter = reader.ReadDouble();
                var speed = reader.ReadVarLong();
                return new LerpSize(oldDiameter, newDiameter, speed);
            }

            case 2:
            {
                var x = reader.ReadDouble();
                var z = reader.ReadDouble();
                return new SetCenter(x, z);
            }

            case 3:
            {
                var x = reader.ReadDouble();
                var z = reader.ReadDouble();
                var oldDiameter = reader.ReadDouble();
                var newDiameter = reader.ReadDouble();
                var speed = reader.ReadVarLong();
                var portalTeleportBoundary = reader.ReadVarInt();
                var warningTime = reader.ReadVarInt();
                var warningBlocks = reader.ReadVarInt();
                return new Initialize(x, z, oldDiameter, newDiameter, speed, portalTeleportBoundary, warningTime, warningBlocks);
            }

            case 4:
            {
                var warningTime = reader.ReadVarInt();
                return new SetWarningTime(warningTime);
            }

            case 5:
            {
                var warningBlocks = reader.ReadVarInt();
                return new SetWarningBlocks(warningBlocks);
            }
        }

        throw new System.NotSupportedException($"WorldBorderAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderAction>(protocolVersion);
        switch (this)
        {
            case SetSize arm:
            {
                double Diameter = arm.Diameter;
                writer.WriteDouble(Diameter);
                return;
            }

            case LerpSize arm:
            {
                double OldDiameter = arm.OldDiameter;
                double NewDiameter = arm.NewDiameter;
                long Speed = arm.Speed;
                writer.WriteDouble(OldDiameter);
                writer.WriteDouble(NewDiameter);
                writer.WriteVarLong(Speed);
                return;
            }

            case SetCenter arm:
            {
                double X = arm.X;
                double Z = arm.Z;
                writer.WriteDouble(X);
                writer.WriteDouble(Z);
                return;
            }

            case Initialize arm:
            {
                double X = arm.X;
                double Z = arm.Z;
                double OldDiameter = arm.OldDiameter;
                double NewDiameter = arm.NewDiameter;
                long Speed = arm.Speed;
                int PortalTeleportBoundary = arm.PortalTeleportBoundary;
                int WarningTime = arm.WarningTime;
                int WarningBlocks = arm.WarningBlocks;
                writer.WriteDouble(X);
                writer.WriteDouble(Z);
                writer.WriteDouble(OldDiameter);
                writer.WriteDouble(NewDiameter);
                writer.WriteVarLong(Speed);
                writer.WriteVarInt(PortalTeleportBoundary);
                writer.WriteVarInt(WarningTime);
                writer.WriteVarInt(WarningBlocks);
                return;
            }

            case SetWarningTime arm:
            {
                int WarningTime = arm.WarningTime;
                writer.WriteVarInt(WarningTime);
                return;
            }

            case SetWarningBlocks arm:
            {
                int WarningBlocks = arm.WarningBlocks;
                writer.WriteVarInt(WarningBlocks);
                return;
            }
        }

        throw new System.NotSupportedException($"WorldBorderAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        switch (this)
        {
            case SetSize _:
                return 0;
            case LerpSize _:
                return 1;
            case SetCenter _:
                return 2;
            case Initialize _:
                return 3;
            case SetWarningTime _:
                return 4;
            case SetWarningBlocks _:
                return 5;
        }

        throw new System.NotSupportedException($"WorldBorderAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }
}
