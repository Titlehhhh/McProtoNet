// EXPERIMENT (McProtoNet.Next) — measurement 1: what one idle connection costs in
// managed memory. Requirement source: docs/design/transport-requirements-2026-08-10.md,
// "вес соединения" — the owner wants many bots on one machine.
//
// Method: create N clients over streams that never deliver a byte, force a full GC,
// and diff the managed heap. The cost of the stub streams themselves is measured
// separately and subtracted, so the number is the client machinery only: two pipes,
// two pump state machines, the semaphore, the CTS, and the receive segment the fill
// pump rents before its first read.
//
// Not included, on purpose: the socket and NetworkStream. Those are the same whatever
// shape the write door takes, so they cannot influence the decision this measures.

namespace McProtoNet.Next.Measurements;

internal static class ConnectionCost
{
    public static async Task RunAsync()
    {
        Console.WriteLine("== Замер 1: цена соединения в покое ==");
        Console.WriteLine("(управляемая память; сокет и NetworkStream не входят)");
        Console.WriteLine();

        await MeasureAsync(32, report: false);   // прогрев JIT и пулов

        foreach (int count in new[] { 100, 1000 })
            await MeasureAsync(count, report: true);
    }

    private static async Task MeasureAsync(int count, bool report)
    {
        var streams = new IdleStream[count];
        var clients = new MinecraftClient[count];

        long baseline = Snapshot();
        for (int i = 0; i < count; i++)
            streams[i] = new IdleStream();
        long afterStreams = Snapshot();

        var options = new MinecraftClientOptions { LeaveOpen = true };
        for (int i = 0; i < count; i++)
            clients[i] = MinecraftClient.Create(streams[i], options);

        // Дать насосам дойти до первого GetMemory/ReadAsync — именно там
        // арендуется сегмент приёма, и это заметная часть цены.
        await Task.Delay(500).ConfigureAwait(false);
        long afterClients = Snapshot();

        GC.KeepAlive(streams);
        GC.KeepAlive(clients);

        if (report)
        {
            double perStream = (afterStreams - baseline) / (double)count;
            double perClient = (afterClients - afterStreams) / (double)count;
            Console.WriteLine($"соединений: {count}");
            Console.WriteLine($"  заглушка потока : {perStream,10:N0} Б/шт");
            Console.WriteLine($"  клиент          : {perClient,10:N0} Б/шт");
            Console.WriteLine($"  на 1000 ботов   : {perClient * 1000 / (1024 * 1024),10:N1} МиБ");
            Console.WriteLine($"  на 10000 ботов  : {perClient * 10000 / (1024 * 1024),10:N1} МиБ");
            Console.WriteLine();
        }

        // Синхронный Dispose: он не ждёт насосы, а насосы висят в чтении навсегда.
        foreach (MinecraftClient client in clients)
            client.Dispose();
    }

    private static long Snapshot()
    {
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        return GC.GetTotalMemory(forceFullCollection: false);
    }

    /// <summary>
    ///     A stream that accepts writes and never delivers a read. One shared never-completing
    ///     task backs every read, so the stub adds no per-connection allocation of its own.
    /// </summary>
    private sealed class IdleStream : Stream
    {
        private static readonly Task<int> NeverCompletes =
            new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously).Task;

        public override bool CanRead => true;
        public override bool CanWrite => true;
        public override bool CanSeek => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => new(NeverCompletes);

        public override ValueTask WriteAsync(
            ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
