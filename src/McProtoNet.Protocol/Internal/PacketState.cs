namespace McProtoNet.Protocol;

public enum PacketState
{
    Status,
    Handshaking,
    Configuration,
    Login,
    Play
}