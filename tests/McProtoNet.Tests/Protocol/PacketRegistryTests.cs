using McProtoNet.Primitives;
using McProtoNet.Protocol;
namespace McProtoNet.Tests.Protocol;

/// <summary>
///     The registry answers id -> ordinal from flat tables indexed by arithmetic over
///     <c>(int)phase</c>, <c>(int)direction</c> and the protocol version. The generator builds those
///     tables from its own idea of the enum order, so a reordered <see cref="PacketPhase" /> or
///     <see cref="PacketDirection" /> would mismap every id without any compiler complaint. These
///     tests cross-check the hot lookup against the cold descriptor catalogs, which carry the
///     (phase, direction, ordinal, id-range) facts independently.
/// </summary>
public class PacketRegistryTests
{
    private static readonly PacketPhase[] Phases = Enum.GetValues<PacketPhase>();

    private static readonly PacketDirection[] Directions = Enum.GetValues<PacketDirection>();

    private const int FirstProtocol = 735;
    private const int LastProtocol = 776;

    public static TheoryData<PacketPhase, PacketDirection> Catalogs()
    {
        var data = new TheoryData<PacketPhase, PacketDirection>();
        foreach (var phase in Phases)
        foreach (var direction in Directions)
            data.Add(phase, direction);

        return data;
    }

    [Theory]
    [MemberData(nameof(Catalogs))]
    public void TryGetOrdinal_AgreesWithTheDescriptorCatalog(PacketPhase phase, PacketDirection direction)
    {
        var catalog = PacketRegistry.Catalog(phase, direction);

        for (var ordinal = 0; ordinal < catalog.Length; ordinal++)
        {
            var descriptor = catalog[ordinal];

            Assert.Equal(phase, descriptor.Identity.Phase);
            Assert.Equal(direction, descriptor.Identity.Direction);
            Assert.Equal(ordinal, descriptor.Identity.Ordinal);

            foreach (var range in descriptor.Ids)
            for (var pv = range.FromPv; pv <= range.ToPv; pv++)
            {
                Assert.True(
                    PacketRegistry.TryGetOrdinal(range.Id, pv, phase, direction, out var found),
                    $"{descriptor.Identity.Key}: id 0x{range.Id:X2} on protocol {pv} did not resolve.");

                Assert.Equal(descriptor.Identity.Ordinal, found);
            }
        }
    }

    [Theory]
    [MemberData(nameof(Catalogs))]
    public void TryGetOrdinal_IdThatNoDescriptorClaims_ReturnsFalse(PacketPhase phase, PacketDirection direction)
    {
        var catalog = PacketRegistry.Catalog(phase, direction);

        for (var pv = FirstProtocol; pv <= LastProtocol; pv++)
        {
            var claimed = new HashSet<int>();
            foreach (var descriptor in catalog)
            foreach (var range in descriptor.Ids)
                if (pv >= range.FromPv && pv <= range.ToPv)
                    claimed.Add(range.Id);

            for (var id = 0; id <= 0xFF; id++)
            {
                if (claimed.Contains(id))
                    continue;

                Assert.False(
                    PacketRegistry.TryGetOrdinal(id, pv, phase, direction, out _),
                    $"{phase}/{direction}: id 0x{id:X2} on protocol {pv} resolved but no descriptor claims it.");
            }
        }
    }

    [Theory]
    [MemberData(nameof(Catalogs))]
    public void TryGetOrdinal_ProtocolVersionOutsideTheKnownRange_ReturnsFalse(
        PacketPhase phase,
        PacketDirection direction)
    {
        foreach (var pv in new[] { int.MinValue, -1, 0, FirstProtocol - 1, LastProtocol + 1, int.MaxValue })
        for (var id = 0; id <= 0xFF; id++)
            Assert.False(PacketRegistry.TryGetOrdinal(id, pv, phase, direction, out _));
    }

    /// <summary>A phase or direction outside its enum can only arrive by a cast, but the lookup
    /// indexes flat tables by those numbers, so an unbounded one would read another catalog's row
    /// and hand back a confidently wrong ordinal. Direction is the sharp case: the slot is
    /// phase * DirectionCount + direction, so direction 2 with phase 0 lands exactly on phase 1.</summary>
    [Theory]
    [InlineData(5, 0)]
    [InlineData(200, 0)]
    [InlineData(0, 2)]
    [InlineData(0, 200)]
    [InlineData(200, 200)]
    public void TryGetOrdinal_PhaseOrDirectionOutsideTheEnum_ReturnsFalse(byte phase, byte direction)
    {
        for (var pv = FirstProtocol; pv <= LastProtocol; pv++)
        for (var id = 0; id <= 0xFF; id++)
            Assert.False(PacketRegistry.TryGetOrdinal(id, pv, (PacketPhase)phase, (PacketDirection)direction, out _));
    }

    [Fact]
    public void TryResolve_PhaseOrDirectionOutsideTheEnum_ReturnsFalseWithoutThrowing()
    {
        var exception = Record.Exception(() =>
        {
            Assert.False(PacketRegistry.TryResolve(0, 772, (PacketPhase)200, PacketDirection.Clientbound, out _));
            Assert.False(PacketRegistry.TryResolve(0, 772, PacketPhase.Play, (PacketDirection)200, out _));
        });

        Assert.Null(exception);
    }

    /// <summary>The dispatcher shares the same lookup, so a bogus phase or direction must come out
    /// as the normal "no mapping" condition — Unknown — and never as a throw.</summary>
    [Fact]
    public void Dispatch_PhaseOrDirectionOutsideTheEnum_ReachesUnknownWithoutThrowing()
    {
        var raw = new IncomingPacket(0x26, new byte[8]);

        var visitor = new CountingVisitor();
        PacketFlow.Dispatch(in raw, 772, (PacketPhase)200, PacketDirection.Clientbound, ref visitor);
        PacketFlow.Dispatch(in raw, 772, PacketPhase.Play, (PacketDirection)200, ref visitor);

        Assert.Equal(0, visitor.Visited);
        Assert.Equal(2, visitor.Unknowns);

        var tryVisitor = new CountingVisitor();
        Assert.True(PacketFlow.TryDispatch(in raw, 772, (PacketPhase)200, PacketDirection.Clientbound, ref tryVisitor,
            out var error));
        Assert.Equal(DecodeError.None, error);
        Assert.Equal(1, tryVisitor.Unknowns);
    }

    private struct CountingVisitor : IPacketVisitor
    {
        public int Visited;
        public int Unknowns;

        public void Visit<T>(T packet) where T : class, IPacket<T> => Visited++;

        public void Unknown(in IncomingPacket raw) => Unknowns++;
    }

    [Fact]
    public void CatalogCount_MatchesTheEnumsItIndexes()
    {
        Assert.Equal(Phases.Length, PacketRegistry.PhaseCount);
        Assert.Equal(Directions.Length, PacketRegistry.DirectionCount);
        Assert.Equal(PacketRegistry.PhaseCount * PacketRegistry.DirectionCount, PacketRegistry.CatalogCount);
    }

    [Fact]
    public void TryResolve_RoundTripsEveryDescriptorOfEveryCatalog()
    {
        foreach (var phase in Phases)
        foreach (var direction in Directions)
        {
            var catalog = PacketRegistry.Catalog(phase, direction);

            foreach (var descriptor in catalog)
            foreach (var range in descriptor.Ids)
            {
                Assert.True(PacketRegistry.TryResolve(range.Id, range.FromPv, phase, direction, out var resolved));
                Assert.Equal(descriptor.Identity, resolved.Identity);
            }
        }
    }
}
