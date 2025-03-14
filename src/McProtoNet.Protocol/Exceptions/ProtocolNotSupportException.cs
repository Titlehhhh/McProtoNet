﻿namespace McProtoNet.Protocol;

/// <summary>
/// Exception thrown when a protocol is not supported
/// </summary>
public class ProtocolNotSupportException(string packetName, int protocolVersion) : Exception
{
    public string PacketName { get; } = packetName;
    public int ProtocolVersion { get; } = protocolVersion;
}