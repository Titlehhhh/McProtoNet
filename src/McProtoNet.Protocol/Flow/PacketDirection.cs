namespace McProtoNet.Protocol;

/// <summary>
/// Specifies the direction a packet travels in.
/// </summary>
public enum PacketDirection : byte
{
    /// <summary>The packet is sent by the server and received by the client.</summary>
    Clientbound,

    /// <summary>The packet is sent by the client and received by the server.</summary>
    Serverbound
}
