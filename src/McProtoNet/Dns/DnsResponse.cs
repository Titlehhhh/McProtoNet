namespace McProtoNet;

/// <summary>Holds the header bits and the SRV records of a parsed DNS answer.</summary>
internal readonly struct DnsResponse
{
    public DnsResponse(bool truncated, int responseCode, IReadOnlyList<SrvResult> records)
    {
        Truncated = truncated;
        ResponseCode = responseCode;
        Records = records;
    }

    /// <summary>Gets the TC bit: the answer did not fit a datagram and must be asked again over TCP.</summary>
    public bool Truncated { get; }

    /// <summary>Gets the RCODE of the answer.</summary>
    public int ResponseCode { get; }

    public IReadOnlyList<SrvResult> Records { get; }
}
