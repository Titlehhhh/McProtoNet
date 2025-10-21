using System.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Net;

public readonly struct NewInputPacket
{
    private readonly IPacketSource _source;
    private readonly int _version;


    public int Id => _source.GetId(_version);

    public ReadOnlySequence<byte> Data => _source.GetData(_version);

    public NewInputPacket(IPacketSource source, int version)
    {
        _source = source;
        _version = version;
    }
    public long FullLength => Id.GetVarIntLength() + Data.Length;
}