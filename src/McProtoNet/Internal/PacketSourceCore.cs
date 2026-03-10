using System.Buffers;
using System.Diagnostics;
using McProtoNet.Net;

namespace McProtoNet.Internal;


internal class PacketSourceCore : IPacketSource
{
    public int Id;
    public ReadOnlySequence<byte> Data;

    public void Reset()
    {
        unchecked
        {
            _version++;
        }
        Id = -1;
        Data = ReadOnlySequence<byte>.Empty;
    }

    private int _version; // it's not Minecraft protocol version

    public int Version => _version;

    public int GetId(int token)
    {
        if (_version != token)
            throw new InvalidOperationException("Packet returned to pool");
        var id = Id;
        Debug.Assert(id >= 0, 
            $"PacketSourceCore.GetId called with invalid id {id}");
        return id;
    }

    public ReadOnlySequence<byte> GetData(int token)
    {
        if (_version != token)
            throw new InvalidOperationException("Packet returned to pool");

        return Data;
    }

    
}