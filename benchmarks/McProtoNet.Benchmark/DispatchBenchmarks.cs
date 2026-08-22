using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using McProtoNet.Primitives;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Play.Clientbound;

namespace McProtoNet.Benchmark;

public enum CorpusKind
{
    Small,
    Wide
}

/// <summary>The receive path a consumer actually runs: a raw packet in, the matching
/// <c>On&lt;Name&gt;</c> out. The corpus is built once and holds no sockets, so what is measured is
/// the registry lookup, the typed read and the handler call, and nothing else.</summary>
[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class DispatchBenchmarks
{
    private IncomingPacket[] _corpus = [];
    private DirectHandler _direct = null!;
    private LegacyRouteHandler _visitor = new();

    [Params(735, 772)] public int ProtocolVersion { get; set; }

    /// <summary>Small = only the short, frequent play packets, where the per-packet dispatch
    /// overhead is the bulk of the time. Wide adds two byte-array payloads: enough to represent a
    /// wide body without turning the measurement into a memcpy benchmark.</summary>
    [Params(CorpusKind.Small, CorpusKind.Wide)]
    public CorpusKind Corpus { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _direct = new DirectHandler();
        _visitor = new LegacyRouteHandler();
        _corpus = BuildCorpus(ProtocolVersion, Corpus);
    }

    /// <summary>The route the generated handler takes today.</summary>
    [Benchmark]
    public long Handler()
    {
        var handler = _direct;
        var pv = ProtocolVersion;
        var corpus = _corpus;
        for (var i = 0; i < corpus.Length; i++)
            handler.HandleAsync(in corpus[i], pv).GetAwaiter().GetResult();

        return handler.Sink;
    }

    /// <summary>The route the generated handler used to take, kept as a live baseline so the
    /// difference stays reproducible from what is in the tree rather than from a claim in a
    /// comment. It is driven exactly like the arm above — same corpus, same ValueTask round trip,
    /// same virtual On* call — so the only difference measured is the route itself.</summary>
    [Benchmark(Baseline = true)]
    public long VisitorRoute()
    {
        var handler = _visitor;
        var pv = ProtocolVersion;
        var corpus = _corpus;
        for (var i = 0; i < corpus.Length; i++)
            handler.HandleAsync(in corpus[i], pv).GetAwaiter().GetResult();

        return handler.Sink;
    }

    private static IncomingPacket[] BuildCorpus(int pv, CorpusKind kind)
    {
        var list = new List<IncomingPacket>();

        // Frequent, small play clientbound packets, repeated to weight the mix the way a
        // live stream does.
        for (var i = 0; i < 8; i++)
        {
            Add(list, pv, new KeepAlivePacket(0x0123456789ABCDEFL ^ i));
            Add(list, pv, new RelEntityMovePacket(1000 + i, 12, -34, 56, true));
            Add(list, pv, new EntityLookPacket(2000 + i, 17, -42, false));
            Add(list, pv, new EntityMoveLookPacket(3000 + i, 5, 6, 7, 8, 9, true));
            Add(list, pv, new EntityHeadRotationPacket(4000 + i, 33));
            Add(list, pv, new BlockChangePacket(new Position(100 + i, 64, -200), 12345));
        }

        // Wide keeps the same dispatch count as Small and adds bodies wide enough that the read,
        // not the dispatch, dominates each of them — the two corpora bracket the realistic mix.
        if (kind == CorpusKind.Wide)
            for (var i = 0; i < 8; i++)
            {
                Add(list, pv, new CustomPayloadPacket("minecraft:brand", FillerBytes(256 + i)));
                Add(list, pv, new CustomPayloadPacket("minecraft:register", FillerBytes(512 + i)));
                Add(list, pv, new CustomPayloadPacket("minecraft:debug/paths", FillerBytes(1024 + i)));
            }

        return list.ToArray();
    }

    private static byte[] FillerBytes(int length)
    {
        var data = new byte[length];
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(i * 31 + 7);

        return data;
    }

    private static void Add<T>(List<IncomingPacket> sink, int pv, T packet) where T : class, IPacket<T>
    {
        if (!T.TryGetPacketId(pv, out var id))
            throw new InvalidOperationException($"{typeof(T).Name} has no id on protocol {pv}.");

        var writer = new MinecraftPrimitiveWriter();
        packet.Write(writer, pv);
        using var written = writer.GetWrittenMemory();
        sink.Add(new IncomingPacket(id, written.Memory.ToArray()));
    }

    private static InvalidOperationException NotDispatched(in IncomingPacket raw)
    {
        return new InvalidOperationException($"corpus packet id 0x{raw.Id:X2} did not dispatch.");
    }

    // Both consumers below do work the JIT cannot delete, so the decode survives dead-code
    // elimination and the measurement covers the whole path, not an empty call. They add the same
    // quantities, so the arms differ only in how the packet gets to them.

    /// <summary>The shape the generated handler had before: the handler is itself the visitor,
    /// <c>HandleAsync</c> parks the result in a field because <c>Visit&lt;T&gt;</c> has nowhere to
    /// return it, and each packet costs a static identity read (with T a reference type the
    /// dispatch runs as shared generic code, so that is a runtime generic-dictionary lookup that
    /// rebuilds a PacketIdentity every packet), a phase switch, an ordinal switch, a cast and a
    /// virtual call. Kept here, and driven exactly like the live handler, so the two routes stay
    /// comparable from inside the tree.</summary>
    private class LegacyRouteHandler : IPacketVisitor
    {
        public long Sink;
        private ValueTask _pending;

        public ValueTask HandleAsync(in IncomingPacket raw, int protocolVersion)
        {
            _pending = default;
            var self = this;
            PacketFlow.Dispatch(in raw, protocolVersion, PacketPhase.Play, PacketDirection.Clientbound, ref self);
            return _pending;
        }

        void IPacketVisitor.Visit<T>(T packet)
        {
            var identity = T.Identity;
            switch (identity.Phase)
            {
                case PacketPhase.Play:
                    switch (identity.Ordinal)
                    {
                        case 8:
                            _pending = OnBlockChange((BlockChangePacket)(object)packet);
                            return;
                        case 22:
                            _pending = OnCustomPayload((CustomPayloadPacket)(object)packet);
                            return;
                        case 33:
                            _pending = OnEntityHeadRotation((EntityHeadRotationPacket)(object)packet);
                            return;
                        case 34:
                            _pending = OnEntityLook((EntityLookPacket)(object)packet);
                            return;
                        case 36:
                            _pending = OnEntityMoveLook((EntityMoveLookPacket)(object)packet);
                            return;
                        case 50:
                            _pending = OnKeepAlive((KeepAlivePacket)(object)packet);
                            return;
                        case 71:
                            _pending = OnRelEntityMove((RelEntityMovePacket)(object)packet);
                            return;
                    }

                    break;
            }

            throw new InvalidOperationException($"corpus packet {identity.Key} has no visitor case.");
        }

        protected virtual ValueTask OnKeepAlive(KeepAlivePacket packet)
        {
            Sink += packet.KeepAliveId;
            return default;
        }

        protected virtual ValueTask OnRelEntityMove(RelEntityMovePacket packet)
        {
            Sink += packet.EntityId + packet.Dx;
            return default;
        }

        protected virtual ValueTask OnEntityLook(EntityLookPacket packet)
        {
            Sink += packet.EntityId + packet.Yaw;
            return default;
        }

        protected virtual ValueTask OnEntityMoveLook(EntityMoveLookPacket packet)
        {
            Sink += packet.EntityId + packet.Dz;
            return default;
        }

        protected virtual ValueTask OnEntityHeadRotation(EntityHeadRotationPacket packet)
        {
            Sink += packet.EntityId + packet.HeadYaw;
            return default;
        }

        protected virtual ValueTask OnBlockChange(BlockChangePacket packet)
        {
            Sink += packet.Type;
            return default;
        }

        protected virtual ValueTask OnCustomPayload(CustomPayloadPacket packet)
        {
            Sink += packet.Data.Length + packet.Channel.Length;
            return default;
        }

        void IPacketVisitor.Unknown(in IncomingPacket raw) => throw NotDispatched(in raw);
    }

    private sealed class DirectHandler : ClientboundHandler
    {
        public long Sink;

        public DirectHandler() => Phase = PacketPhase.Play;

        protected override ValueTask OnKeepAlive(KeepAlivePacket packet)
        {
            Sink += packet.KeepAliveId;
            return default;
        }

        protected override ValueTask OnRelEntityMove(RelEntityMovePacket packet)
        {
            Sink += packet.EntityId + packet.Dx;
            return default;
        }

        protected override ValueTask OnEntityLook(EntityLookPacket packet)
        {
            Sink += packet.EntityId + packet.Yaw;
            return default;
        }

        protected override ValueTask OnEntityMoveLook(EntityMoveLookPacket packet)
        {
            Sink += packet.EntityId + packet.Dz;
            return default;
        }

        protected override ValueTask OnEntityHeadRotation(EntityHeadRotationPacket packet)
        {
            Sink += packet.EntityId + packet.HeadYaw;
            return default;
        }

        protected override ValueTask OnBlockChange(BlockChangePacket packet)
        {
            Sink += packet.Type;
            return default;
        }

        protected override ValueTask OnCustomPayload(CustomPayloadPacket packet)
        {
            Sink += packet.Data.Length + packet.Channel.Length;
            return default;
        }

        protected override ValueTask OnUnknown(in IncomingPacket raw) => throw NotDispatched(in raw);
    }
}
