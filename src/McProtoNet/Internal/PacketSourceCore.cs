using System.Buffers;
using McProtoNet.Net;

namespace McProtoNet.Internal;


internal class PacketSourceCore : IPacketSource
{
    public int Id;
    public ReadOnlySequence<byte> Data;

    public void IncrementVersion()
    {
        unchecked
        {
            _version++;
        }
    }

    private int _version; // it's not Minecraft protocol version

    public int Version => _version;

    public int GetId(int token)
    {
        if (_version != token)
            throw new InvalidOperationException("Packet returned to pool");
        return Id;
    }

    public ReadOnlySequence<byte> GetData(int token)
    {
        if (_version != token)
            throw new InvalidOperationException("Packet returned to pool");

        return Data;
    }

    
}