using McProtoNet.Primitives;
using McProtoNet.Protocol;

namespace McProtoNet.Scenarios;

/// <summary>Two visitors behind one <see cref="IPacketVisitor" />; see <see cref="PacketWait.Tee{TA,TB}" />.</summary>
public readonly struct TeeVisitor<TA, TB>(TA a, TB b) : IPacketVisitor
    where TA : IPacketVisitor where TB : IPacketVisitor
{
    public void Visit<T>(T packet) where T : class, IPacket<T>
    {
        a.Visit(packet);
        b.Visit(packet);
    }

    public void Unknown(in IncomingPacket raw)
    {
        a.Unknown(in raw);
        b.Unknown(in raw);
    }
}
