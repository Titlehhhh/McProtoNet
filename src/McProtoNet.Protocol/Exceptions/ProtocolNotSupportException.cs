namespace McProtoNet.Protocol;

public class ProtocolNotSupportException(string packetName, int protocolVersion) : Exception;