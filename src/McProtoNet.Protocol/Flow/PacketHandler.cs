namespace McProtoNet.Protocol;

/// <summary>
/// Represents the method that handles a decoded packet of the type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The packet type that is handled.</typeparam>
/// <param name="packet">The decoded packet.</param>
public delegate void PacketHandler<T>(T packet) where T : class, IPacket<T>;
