namespace McProtoNet.Protocol;

public static class PacketFactory
{
    public static IPacket? Create(int hexId, int protocolVersion, PacketState state, PacketDirection direction)
    {
        if (PacketIdHelper.TryGetFactory(hexId, protocolVersion, state, direction, out var factory))
            return factory!();
        return null;
    }
}
