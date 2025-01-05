internal class HandshakePacket
{
    public int ProtocolVersion { get; set; }
    public string Address { get; set; }
    public ushort Port { get; set; }
    public int NextState { get; set; }

    public override string ToString()
    {
        return
            $"{nameof(ProtocolVersion)}: {ProtocolVersion}, {nameof(Address)}: {Address}, {nameof(Port)}: {Port}, {nameof(NextState)}: {NextState}";
    }
}