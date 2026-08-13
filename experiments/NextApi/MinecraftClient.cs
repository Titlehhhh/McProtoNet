// EXPERIMENT (McProtoNet.Next): a new client API next to the old one. The old
// transport keeps living as it is; nothing here is wired into src/**.
//
// Design sources:
//   docs/design/transport-naming-2026-08-10.md    — doors per floor, agreed surface
//   docs/design/transport-api-2026-08-10.md       — floor model
//   docs/design/transport-hardening-plan-2026-08-10.md §1 — defects that MUST NOT
//     be reproduced: ContinueWith chains, encryption wrappers with a second pipe,
//     unvalidated lengths from the wire, unverified decompression.
//
// ---------------------------------------------------------------------------
// ASSUMED SURFACE OF THE Frames/ AUTHOR'S TYPES (written in parallel; the
// integrator reconciles). RawFrame / FrameEnvelope / FrameOptions /
// FrameCompression shapes are agreed; FrameReader / FrameWriter are assumed as:
//
//   public sealed class FrameReader
//   {
//       public FrameReader(PipeReader input);
//       public int CompressionThreshold { get; set; }   // -1 = off
//
//       // Hacker door: frames as on the wire, payload NOT decompressed.
//       // Frame window is valid until the next MoveNextAsync.
//       public IAsyncEnumerable<RawFrame> ReadFramesAsync(
//           CancellationToken cancellationToken = default);
//
//       // Packet-floor feed: the compression envelope opened and VERIFIED
//       // (declared lengths validated, decompressed size checked against the
//       // claim — hardening plan К1/К2). Yields one decompressed payload per
//       // frame; the sequence window is valid until the next MoveNextAsync.
//       // Pipe completed cleanly at a frame boundary => enumeration ends;
//       // completed mid-frame => truncation error is thrown.
//       public IAsyncEnumerable<ReadOnlySequence<byte>> ReadPayloadsAsync(
//           CancellationToken cancellationToken = default);
//   }
//
//   public sealed class FrameWriter
//   {
//       public FrameWriter(PipeWriter output);
//       public int CompressionThreshold { get; set; }   // -1 = off
//
//       // Writes one frame (length prefix + compression envelope per options)
//       // and flushes; returns the pipe's FlushResult so the caller can detect
//       // a completed (closed) transport. Does NOT retain the payload after the
//       // returned task completes (the caller may return a rented buffer).
//       public ValueTask<FlushResult> SendFrameAsync(
//           ReadOnlyMemory<byte> payload,
//           FrameOptions options = default,
//           CancellationToken cancellationToken = default);
//   }
// ---------------------------------------------------------------------------

#nullable enable

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Net;

namespace McProtoNet.Next;

/// <summary>
/// Experimental Minecraft protocol client: a facade over System.IO.Pipelines with three
/// floors of doors — packets (<see cref="ReadPacketsAsync"/>/<see cref="SendPacketAsync"/>),
/// frames (<see cref="ReadFramesAsync"/>/<see cref="SendFrameAsync"/>) and raw bytes
/// (<see cref="SendRawBytesAsync"/>).
/// </summary>
/// <remarks>
/// <para>
/// Two internal pumps move bytes between the underlying stream and the pipes: a fill pump
/// (stream → receive pipe, decrypting in place) and a drain pump (send pipe → stream,
/// encrypting on the way out). Both are plain async methods; their failures surface
/// through <see cref="Completion"/>.
/// </para>
/// <para>
/// Threading model: at most one read enumeration (packets or frames) may be active at a
/// time — the transport has a single read cursor. Sends from multiple tasks are
/// serialized on an internal gate. <see cref="EnableEncryption(ReadOnlySpan{byte})"/> and
/// the <see cref="CompressionThreshold"/> setter synchronize with the pumps and can wait
/// for a send already in flight; prefer
/// <see cref="EnableEncryptionAsync(ReadOnlyMemory{byte}, CancellationToken)"/> where a
/// wait matters.
/// </para>
/// <para>
/// Lifetime: there is no separate <c>Close</c>/<c>Disconnect</c> — disposing the client is
/// the close operation (a deliberate surface decision, recorded here). Prefer
/// <see cref="DisposeAsync"/>.
/// </para>
/// <para>
/// Hot-path window rule: <see cref="InputPacket.Data"/> and <see cref="RawFrame.Payload"/>
/// are windows into pipe-owned memory, valid only until the next iteration of the
/// enumeration that produced them. Decode before the next <c>await</c>; never store them.
/// </para>
/// </remarks>
public sealed class MinecraftClient : IAsyncDisposable, IDisposable
{
    private const int MaxVarIntLength = 5;
    private const int ReadBufferHint = 8192;     // hint for PipeWriter.GetMemory in the fill pump
    private const int EncryptChunkSize = 8192;   // scratch size for the encrypting drain path

    private readonly Stream _stream;
    private readonly bool _leaveOpen;
    private readonly Pipe _receivePipe;
    private readonly Pipe _sendPipe;
    private readonly FrameReader _frameReader;
    private readonly FrameWriter _frameWriter;

    // Send serialization. SemaphoreSlim(1,1) is the experiment's pick: concurrent senders
    // queue up in order instead of faulting. Whether concurrent sends should instead be a
    // usage error (the old MinecraftPacketSender threw) is open question Д4 in the
    // hardening plan — this semantic is under discussion, not settled.
    private readonly SemaphoreSlim _sendGate = new(1, 1);

    private readonly CancellationTokenSource _lifetimeCts = new();
    private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Task _pumps;

    private int _compressionThreshold;
    private int _readDoorHeld;   // 0 = free, 1 = a packet/frame enumeration is active
    private int _disposeState;   // 0 = live, 1 = disposed

    // Encryption state. _decryptor is installed by EnableEncryption and sampled once per
    // fill-pump iteration (never mid-chunk). _encryptor belongs to the drain pump: it is
    // assigned only on the pump's own execution path (TryPromotePendingEncryptor), at an
    // iteration boundary with a verifiably empty send pipe — the "flag + barrier" that
    // the old wrapper-pipe design never had.
    private volatile PacketCipher? _decryptor;
    private PacketCipher? _encryptor;
    private PacketCipher? _pendingEncryptor;
    private volatile TaskCompletionSource? _encryptorInstalled;
    private volatile bool _isEncrypted;
    private IDisposable? _cipherOwner;

    private MinecraftClient(Stream stream, MinecraftClientOptions options, bool leaveOpen)
    {
        _stream = stream;
        _leaveOpen = leaveOpen;
        _compressionThreshold = options.CompressionThreshold;

        // Integrator note: the receive pipe must not pause the fill pump while one frame is
        // being buffered — the frame layer holds up to RawFrame.MaxFrameSize (8 MiB) without
        // consuming a byte, while the default pauseWriterThreshold is 64 KiB: any frame above
        // it would deadlock (the audit's >128 KiB trap). But an UNBOUNDED pipe is the opposite
        // defect (hardening plan §1's DoS family): a hostile server against a slow consumer
        // would grow the pipe without limit. The pause threshold is therefore finite and
        // strictly larger than any single frame: 2 × MaxFrameSize. Any one frame still buffers
        // whole (no deadlock), the aggregate is capped, and past the cap the pressure reaches
        // TCP. Resume at one MaxFrameSize keeps the pump from thrashing at the edge. The send
        // pipe keeps default backpressure: the drain pump consumes incrementally.
        var receivePipeOptions = new PipeOptions(
            pauseWriterThreshold: 2L * RawFrame.MaxFrameSize,
            resumeWriterThreshold: RawFrame.MaxFrameSize,
            useSynchronizationContext: false);
        var sendPipeOptions = new PipeOptions(useSynchronizationContext: false);
        _receivePipe = new Pipe(receivePipeOptions);
        _sendPipe = new Pipe(sendPipeOptions);
        _frameReader = new FrameReader(_receivePipe.Reader) { CompressionThreshold = _compressionThreshold };
        _frameWriter = new FrameWriter(_sendPipe.Writer) { CompressionThreshold = _compressionThreshold };

        _pumps = RunPumpsAsync();
    }

    // ------------------------------------------------------------------ creation

    /// <summary>
    /// Connects to a server over TCP and returns a running client.
    /// </summary>
    /// <param name="host">The host name or IP address of the server.</param>
    /// <param name="port">The TCP port of the server, between 1 and 65535.</param>
    /// <param name="options">Optional settings; <see langword="null"/> uses the defaults.</param>
    /// <param name="cancellationToken">A token to cancel the connect attempt.</param>
    /// <returns>A task that completes with the connected client.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="host"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="host"/> is empty, or <paramref name="options"/> holds an invalid value.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is outside 1–65535.</exception>
    /// <exception cref="TimeoutException">The connect attempt exceeded <see cref="MinecraftClientOptions.ConnectTimeout"/>.</exception>
    /// <exception cref="SocketException">The connection could not be established.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled through <paramref name="cancellationToken"/>.</exception>
    public static Task<MinecraftClient> ConnectAsync(
        string host,
        int port,
        MinecraftClientOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(host);
        ArgumentOutOfRangeException.ThrowIfLessThan(port, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(port, 65535);
        MinecraftClientOptions snapshot = Snapshot(options);
        ValidateOptions(snapshot);
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<MinecraftClient>(cancellationToken);
        return ConnectCoreAsync(host, port, snapshot, cancellationToken);
    }

    /// <summary>
    /// Creates a running client over an existing bidirectional stream.
    /// </summary>
    /// <param name="stream">A readable and writable stream carrying the protocol bytes.</param>
    /// <param name="options">Optional settings; <see langword="null"/> uses the defaults.</param>
    /// <returns>The created client; its pumps start immediately.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="stream"/> is not readable and writable, or <paramref name="options"/> holds an invalid value.</exception>
    public static MinecraftClient Create(Stream stream, MinecraftClientOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead || !stream.CanWrite)
            throw new ArgumentException("The stream must be both readable and writable.", nameof(stream));
        MinecraftClientOptions snapshot = Snapshot(options);
        ValidateOptions(snapshot);
        return new MinecraftClient(stream, snapshot, snapshot.LeaveOpen);
    }

    private static async Task<MinecraftClient> ConnectCoreAsync(
        string host, int port, MinecraftClientOptions options, CancellationToken cancellationToken)
    {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = options.NoDelay };
        try
        {
            if (options.ConnectTimeout == Timeout.InfiniteTimeSpan)
            {
                await socket.ConnectAsync(host, port, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(options.ConnectTimeout);
                try
                {
                    await socket.ConnectAsync(host, port, timeoutCts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException(
                        $"Connecting to '{host}:{port}' timed out after {options.ConnectTimeout}.");
                }
            }
        }
        catch
        {
            socket.Dispose();
            throw;
        }
        return new MinecraftClient(new NetworkStream(socket, ownsSocket: true), options, leaveOpen: false);
    }

    private static MinecraftClientOptions Snapshot(MinecraftClientOptions? options) =>
        options is null
            ? new MinecraftClientOptions()
            : new MinecraftClientOptions
            {
                CompressionThreshold = options.CompressionThreshold,
                ConnectTimeout = options.ConnectTimeout,
                NoDelay = options.NoDelay,
                LeaveOpen = options.LeaveOpen,
            };

    private static void ValidateOptions(MinecraftClientOptions options)
    {
        if (options.CompressionThreshold < -1)
            throw new ArgumentException(
                $"{nameof(MinecraftClientOptions.CompressionThreshold)} must be -1 (disabled) or non-negative.",
                nameof(options));
        if (options.ConnectTimeout <= TimeSpan.Zero && options.ConnectTimeout != Timeout.InfiniteTimeSpan)
            throw new ArgumentException(
                $"{nameof(MinecraftClientOptions.ConnectTimeout)} must be positive or Timeout.InfiniteTimeSpan.",
                nameof(options));
    }

    // ------------------------------------------------------------------ properties

    /// <summary>
    /// Gets or sets the compression threshold in bytes; <c>-1</c> disables compression.
    /// </summary>
    /// <value>The current threshold. Frames with a payload at least this long are compressed.</value>
    /// <remarks>
    /// Set it when the server announces Set Compression. Change it from the task that
    /// consumes the read enumeration, between packets — the read side is pull-driven, so
    /// that is a natural frame boundary. The setter takes the send gate so the writer is
    /// never retuned in the middle of a frame; when a send is already in flight, the
    /// setter blocks until that send's network write completes, which under backpressure
    /// is not bounded.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than -1.</exception>
    /// <exception cref="ObjectDisposedException">The client has been disposed (setter only).</exception>
    public int CompressionThreshold
    {
        get => Volatile.Read(ref _compressionThreshold);
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, -1);
            ThrowIfDisposed();
            _sendGate.Wait();
            try
            {
                Volatile.Write(ref _compressionThreshold, value);
                _frameWriter.CompressionThreshold = value;
                _frameReader.CompressionThreshold = value;
            }
            finally
            {
                _sendGate.Release();
            }
        }
    }

    /// <summary>
    /// Gets a value that indicates whether AES/CFB8 encryption has been enabled.
    /// </summary>
    /// <value><see langword="true"/> after a successful <see cref="EnableEncryption(ReadOnlySpan{byte})"/> call.</value>
    public bool IsEncrypted => _isEncrypted;

    /// <summary>
    /// Gets a task that completes when both internal pumps have stopped.
    /// </summary>
    /// <value>
    /// A task that completes successfully after a clean shutdown (disposal, or a clean
    /// server close followed by disposal) and faults with the transport errors of both
    /// pumps — all of them, not just the first — when the connection dies.
    /// </value>
    /// <remarks>
    /// When one pump fails, the other is brought down by cancellation; the read
    /// enumeration may then end with <see cref="OperationCanceledException"/>. Await this
    /// task to observe the root cause.
    /// </remarks>
    public Task Completion => _completion.Task;

    // ------------------------------------------------------------------ encryption

    /// <summary>
    /// Enables AES-128/CFB8 encryption in both directions from a 16-byte shared secret
    /// (the secret serves as both key and IV, as the protocol prescribes).
    /// </summary>
    /// <param name="sharedSecret">The 16-byte shared secret negotiated during login.</param>
    /// <exception cref="ArgumentException"><paramref name="sharedSecret"/> is not exactly 16 bytes.</exception>
    /// <exception cref="InvalidOperationException">Encryption is already enabled.</exception>
    /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
    /// <remarks>
    /// <para>
    /// Call this after awaiting the send of the Encryption Response packet. The method
    /// synchronizes honestly with the pumps: everything queued before the call (the
    /// Encryption Response itself) leaves the machine as plaintext; the cipher applies
    /// from the first byte queued after it. On the receive side the cipher applies to
    /// every byte that arrives from the call onward — safe because the server is silent
    /// between Encryption Request and our Response, so no plaintext can be in flight.
    /// </para>
    /// <para>
    /// This synchronous door blocks the calling thread until the drain pump has written
    /// every previously queued byte to the network; under backpressure that wait is not
    /// bounded and cannot be canceled. Prefer
    /// <see cref="EnableEncryptionAsync(ReadOnlyMemory{byte}, CancellationToken)"/>.
    /// Ciphers run in place at the pump boundary — no wrapper streams, no second pipe
    /// (hardening plan, variant А).
    /// </para>
    /// </remarks>
    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        ThrowIfDisposed();
        ValidateSharedSecret(sharedSecret.Length);
        var (decryptor, encryptor, owner) = CreateAesCfb8Pair(sharedSecret);
        InstallCipher(decryptor, encryptor, owner);
    }

    /// <summary>
    /// Enables AES-128/CFB8 encryption in both directions from a 16-byte shared secret,
    /// waiting asynchronously for the plaintext barrier. The preferred encryption door.
    /// </summary>
    /// <param name="sharedSecret">The 16-byte shared secret negotiated during login.</param>
    /// <param name="cancellationToken">A token to cancel the wait for the barrier.</param>
    /// <returns>A task that completes when encryption is active in both directions.</returns>
    /// <exception cref="ArgumentException"><paramref name="sharedSecret"/> is not exactly 16 bytes.</exception>
    /// <exception cref="InvalidOperationException">Encryption is already enabled.</exception>
    /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled through <paramref name="cancellationToken"/>.</exception>
    /// <remarks>
    /// Semantics match <see cref="EnableEncryption(ReadOnlySpan{byte})"/>: everything
    /// queued before the call leaves as plaintext, the cipher applies from the first byte
    /// after. The wait for the drain pump is awaited, not blocked on. A cancellation that
    /// fires after installation has begun leaves the transport in an indeterminate
    /// encryption state — dispose the client in that case.
    /// </remarks>
    public Task EnableEncryptionAsync(ReadOnlyMemory<byte> sharedSecret, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ValidateSharedSecret(sharedSecret.Length);
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);
        return EnableEncryptionCoreAsync(sharedSecret, cancellationToken);
    }

    private async Task EnableEncryptionCoreAsync(ReadOnlyMemory<byte> sharedSecret, CancellationToken cancellationToken)
    {
        var (decryptor, encryptor, owner) = CreateAesCfb8Pair(sharedSecret.Span);
        await InstallCipherAsync(decryptor, encryptor, owner, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Enables encryption with caller-supplied ciphers. Expert overload.
    /// </summary>
    /// <param name="decryptor">A cipher for inbound bytes.</param>
    /// <param name="encryptor">A cipher for outbound bytes.</param>
    /// <exception cref="ArgumentNullException"><paramref name="decryptor"/> or <paramref name="encryptor"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Encryption is already enabled.</exception>
    /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
    /// <remarks>
    /// The ciphers must map input length 1:1 to output length, like CFB8 (see
    /// <see cref="PacketCipher"/>). The client takes ownership of both instances and
    /// disposes them with itself. Timing and blocking semantics match
    /// <see cref="EnableEncryption(ReadOnlySpan{byte})"/>; prefer
    /// <see cref="EnableEncryptionAsync(PacketCipher, PacketCipher, CancellationToken)"/>.
    /// </remarks>
    public void EnableEncryption(PacketCipher decryptor, PacketCipher encryptor)
    {
        ArgumentNullException.ThrowIfNull(decryptor);
        ArgumentNullException.ThrowIfNull(encryptor);
        ThrowIfDisposed();
        InstallCipher(decryptor, encryptor, owner: null);
    }

    /// <summary>
    /// Enables encryption with caller-supplied ciphers, waiting asynchronously for the
    /// plaintext barrier. Expert overload.
    /// </summary>
    /// <param name="decryptor">A cipher for inbound bytes.</param>
    /// <param name="encryptor">A cipher for outbound bytes.</param>
    /// <param name="cancellationToken">A token to cancel the wait for the barrier.</param>
    /// <returns>A task that completes when encryption is active in both directions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="decryptor"/> or <paramref name="encryptor"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Encryption is already enabled.</exception>
    /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled through <paramref name="cancellationToken"/>.</exception>
    /// <remarks>
    /// Semantics match <see cref="EnableEncryption(PacketCipher, PacketCipher)"/>; the
    /// cancellation caveat of
    /// <see cref="EnableEncryptionAsync(ReadOnlyMemory{byte}, CancellationToken)"/> applies.
    /// </remarks>
    public Task EnableEncryptionAsync(
        PacketCipher decryptor, PacketCipher encryptor, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(decryptor);
        ArgumentNullException.ThrowIfNull(encryptor);
        ThrowIfDisposed();
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);
        return InstallCipherAsync(decryptor, encryptor, owner: null, cancellationToken);
    }

    private static void ValidateSharedSecret(int length)
    {
        if (length != 16)
            throw new ArgumentException(
                "The shared secret must be exactly 16 bytes (AES-128).", "sharedSecret");
    }

    private static (PacketCipher Decryptor, PacketCipher Encryptor, IDisposable? Owner) CreateAesCfb8Pair(
        ReadOnlySpan<byte> sharedSecret)
    {
        // Д5: CFB8 runs on the platform's own Aes.EncryptCfb/DecryptCfb — the whole feedback
        // loop in native crypto (see AesCfb8Cipher.cs). Each direction owns its Aes, so there
        // is no shared owner to hand off. IV = shared secret per the protocol.
        PacketCipher? decryptor = null;
        PacketCipher? encryptor = null;
        try
        {
            decryptor = new AesCfb8Cipher(sharedSecret, iv: sharedSecret, encrypting: false);
            encryptor = new AesCfb8Cipher(sharedSecret, iv: sharedSecret, encrypting: true);
            return (decryptor, encryptor, null);
        }
        catch
        {
            decryptor?.Dispose();
            encryptor?.Dispose();
            throw;
        }
    }

    private void InstallCipher(PacketCipher decryptor, PacketCipher encryptor, IDisposable? owner)
    {
        _sendGate.Wait(); // serialize against senders; waits for a send already in flight
        try
        {
            TaskCompletionSource installed = BeginInstall(decryptor, encryptor, owner);
            // Wait until the pump reports the swap, or until the pumps are dead — then
            // there is nothing left to keep plaintext-ordered.
            Task.WaitAny(installed.Task, _pumps);
            FinishInstall(installed, encryptor);
        }
        finally
        {
            _sendGate.Release();
        }
    }

    private async Task InstallCipherAsync(
        PacketCipher decryptor, PacketCipher encryptor, IDisposable? owner, CancellationToken cancellationToken)
    {
        try
        {
            await _sendGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // Nothing installed yet — the ciphers are still ours to clean up.
            DisposeCipherHandoff(decryptor, encryptor, owner);
            throw;
        }
        try
        {
            TaskCompletionSource installed = BeginInstall(decryptor, encryptor, owner);
            // From this point the client's fields own the ciphers (CleanupCore disposes
            // them). A cancellation of the barrier wait below leaves the transport in an
            // indeterminate encryption state — documented on the public doors.
            await Task.WhenAny(installed.Task, _pumps).WaitAsync(cancellationToken).ConfigureAwait(false);
            FinishInstall(installed, encryptor);
        }
        finally
        {
            _sendGate.Release();
        }
    }

    // Runs the pre-install checks (disposing the handed-off ciphers when they fail) and
    // publishes the ciphers to the pumps. Must be called under the send gate.
    private TaskCompletionSource BeginInstall(PacketCipher decryptor, PacketCipher encryptor, IDisposable? owner)
    {
        try
        {
            ThrowIfDisposed();
            if (_isEncrypted)
                throw new InvalidOperationException("Encryption is already enabled.");
        }
        catch
        {
            DisposeCipherHandoff(decryptor, encryptor, owner);
            throw;
        }

        _cipherOwner = owner;

        // Receive side first. By protocol the server is silent between Encryption
        // Request and our Encryption Response, so no plaintext can be in flight;
        // everything that arrives from now on is ciphertext. Installing the decryptor
        // before the send barrier below closes the loopback race where the server's
        // first encrypted bytes land before this method returns.
        _decryptor = decryptor;

        // Send side. Everything queued before this call (the Encryption Response
        // itself) must leave as plaintext. Hand the cipher to the drain pump and let
        // it promote it only at an iteration boundary with a verifiably empty send
        // pipe — the honest flag + barrier.
        var installed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _encryptorInstalled = installed;
        Volatile.Write(ref _pendingEncryptor, encryptor);
        _sendPipe.Reader.CancelPendingRead(); // nudge a parked pump to run its promotion check
        return installed;
    }

    private void FinishInstall(TaskCompletionSource installed, PacketCipher encryptor)
    {
        if (!installed.Task.IsCompleted)
        {
            // Pumps stopped before promoting. Install directly so IsEncrypted stays
            // truthful; the transport is beyond use and Completion carries the reason.
            Interlocked.Exchange(ref _pendingEncryptor, null);
            _encryptor = encryptor;
        }
        _isEncrypted = true;
    }

    private static void DisposeCipherHandoff(PacketCipher decryptor, PacketCipher encryptor, IDisposable? owner)
    {
        decryptor.Dispose();
        encryptor.Dispose();
        owner?.Dispose();
    }

    // ------------------------------------------------------------------ read doors

    /// <summary>
    /// Reads the incoming stream as packets: the compression envelope is opened and
    /// verified, and the VarInt packet id is split from the body.
    /// </summary>
    /// <param name="cancellationToken">A token to stop the enumeration.</param>
    /// <returns>An async stream of packets, ending when the server closes the connection cleanly.</returns>
    /// <exception cref="ObjectDisposedException">The client has been disposed — before the call, or mid-read (thrown from the enumeration).</exception>
    /// <exception cref="InvalidOperationException">Another read enumeration is already active (thrown on the first iteration).</exception>
    /// <exception cref="InvalidDataException">A frame carried a malformed VarInt packet id.</exception>
    /// <exception cref="ProtocolViolationException">A frame length prefix is malformed or outside [0, <see cref="RawFrame.MaxFrameSize"/>], the connection was cut mid-frame, or a compressed body fails verification against its claimed size.</exception>
    /// <exception cref="OperationCanceledException">The enumeration was canceled through <paramref name="cancellationToken"/>.</exception>
    /// <remarks>
    /// <see cref="InputPacket.Data"/> is a window into transport-owned memory, valid only
    /// until the next iteration — decode it before the next <c>await</c>. A clean server
    /// close at a frame boundary ends the enumeration without an exception; a cut in the
    /// middle of a frame surfaces as a truncation error.
    /// </remarks>
    public IAsyncEnumerable<InputPacket> ReadPacketsAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return ReadPacketsCoreAsync(cancellationToken);
    }

    private async IAsyncEnumerable<InputPacket> ReadPacketsCoreAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        AcquireReadDoor();
        try
        {
            await foreach (ReadOnlySequence<byte> payload in
                _frameReader.ReadPayloadsAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return DecodePacket(payload);
            }
        }
        finally
        {
            ReleaseReadDoor();
        }
    }

    /// <summary>
    /// Reads the incoming stream as raw frames, without opening the compression envelope.
    /// </summary>
    /// <param name="cancellationToken">A token to stop the enumeration.</param>
    /// <returns>An async stream of frames as they appear on the wire.</returns>
    /// <exception cref="ObjectDisposedException">The client has been disposed — before the call, or mid-read (thrown from the enumeration).</exception>
    /// <exception cref="InvalidOperationException">Another read enumeration is already active (thrown on the first iteration).</exception>
    /// <exception cref="ProtocolViolationException">A frame length prefix is malformed or outside [0, <see cref="RawFrame.MaxFrameSize"/>], or the connection was cut mid-frame.</exception>
    /// <exception cref="OperationCanceledException">The enumeration was canceled through <paramref name="cancellationToken"/>.</exception>
    /// <remarks>
    /// The hacker door: garbage payloads are visible and do not kill the loop.
    /// <see cref="RawFrame.Payload"/> is valid only until the next iteration.
    /// </remarks>
    public IAsyncEnumerable<RawFrame> ReadFramesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return ReadFramesCoreAsync(cancellationToken);
    }

    private async IAsyncEnumerable<RawFrame> ReadFramesCoreAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        AcquireReadDoor();
        try
        {
            await foreach (RawFrame frame in
                _frameReader.ReadFramesAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return frame;
            }
        }
        finally
        {
            ReleaseReadDoor();
        }
    }

    private static InputPacket DecodePacket(in ReadOnlySequence<byte> payload)
    {
        var reader = new SequenceReader<byte>(payload);
        int id = 0;
        int shift = 0;
        while (true)
        {
            if (!reader.TryRead(out byte b))
                throw new InvalidDataException("The frame payload ended inside the VarInt packet id.");
            id |= (b & 0x7F) << shift;
            if ((b & 0x80) == 0)
                break;
            shift += 7;
            if (shift >= MaxVarIntLength * 7)
                throw new InvalidDataException("The VarInt packet id is longer than 5 bytes.");
        }
        return new InputPacket(id, payload.Slice(reader.Position));
    }

    private void AcquireReadDoor()
    {
        if (Interlocked.CompareExchange(ref _readDoorHeld, 1, 0) != 0)
            throw new InvalidOperationException(
                "Another ReadPacketsAsync/ReadFramesAsync enumeration is already active. " +
                "The transport has a single read cursor; finish or dispose the previous enumeration first.");
    }

    private void ReleaseReadDoor() => Volatile.Write(ref _readDoorHeld, 0);

    // ------------------------------------------------------------------ send doors

    /// <summary>
    /// Sends one packet: the id is written as a VarInt in front of the body and the whole
    /// thing goes out as a single frame under the current compression settings.
    /// </summary>
    /// <param name="id">The packet id, written as a VarInt.</param>
    /// <param name="body">The packet body. It is not retained after the returned task completes, so a rented buffer may be reused from then on — not earlier: with another send in flight this method waits on the send gate before it copies, so the copy can happen after the call returns.</param>
    /// <param name="cancellationToken">A token to cancel the send.</param>
    /// <returns>A task that completes when the frame has been handed to the transport.</returns>
    /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The transport is closed; await <see cref="Completion"/> for the root cause.</exception>
    /// <exception cref="OperationCanceledException">The send was canceled through <paramref name="cancellationToken"/>.</exception>
    public ValueTask SendPacketAsync(int id, ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return SendPacketCoreAsync(id, body, cancellationToken);
    }

    private async ValueTask SendPacketCoreAsync(int id, ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
    {
        await _sendGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            // Experiment simplification: one rented copy to prepend the VarInt id.
            // TODO(NextApi): a scatter form on FrameWriter (header + body) would remove it.
            byte[] rented = ArrayPool<byte>.Shared.Rent(MaxVarIntLength + body.Length);
            try
            {
                int headerLength = WriteVarInt(rented, id);
                body.Span.CopyTo(rented.AsSpan(headerLength));
                FlushResult flushed = await _frameWriter
                    .SendFrameAsync(rented.AsMemory(0, headerLength + body.Length), default, cancellationToken)
                    .ConfigureAwait(false);
                ThrowIfSendCompleted(flushed);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // An internal cancellation (a dying pump, not the caller's token) must not
            // masquerade as a canceled send — same closed-transport error as below.
            throw NewTransportClosedException();
        }
        finally
        {
            _sendGate.Release();
        }
    }

    /// <summary>
    /// Sends one frame with explicit control over the compression envelope.
    /// </summary>
    /// <param name="payload">The frame payload (packet id + body, or anything at all). Not retained after the call completes.</param>
    /// <param name="options">Envelope control: force or suppress compression, lie about the uncompressed size.</param>
    /// <param name="cancellationToken">A token to cancel the send.</param>
    /// <returns>A task that completes when the frame has been handed to the transport.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="FrameOptions.Compression"/> is not a defined <see cref="FrameCompression"/> value.</exception>
    /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The transport is closed; await <see cref="Completion"/> for the root cause.</exception>
    /// <exception cref="OperationCanceledException">The send was canceled through <paramref name="cancellationToken"/>.</exception>
    /// <remarks>The hacker door: deliberately violating the threshold or the declared size is the point.</remarks>
    public ValueTask SendFrameAsync(
        ReadOnlyMemory<byte> payload, FrameOptions options = default, CancellationToken cancellationToken = default)
    {
        if (options.Compression is < FrameCompression.Auto or > FrameCompression.Never)
            throw new ArgumentOutOfRangeException(nameof(options), options.Compression,
                $"Unknown {nameof(FrameCompression)} value.");
        ThrowIfDisposed();
        return SendFrameCoreAsync(payload, options, cancellationToken);
    }

    private async ValueTask SendFrameCoreAsync(
        ReadOnlyMemory<byte> payload, FrameOptions options, CancellationToken cancellationToken)
    {
        await _sendGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            FlushResult flushed = await _frameWriter.SendFrameAsync(payload, options, cancellationToken).ConfigureAwait(false);
            ThrowIfSendCompleted(flushed);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw NewTransportClosedException();
        }
        finally
        {
            _sendGate.Release();
        }
    }

    /// <summary>
    /// Sends bytes past the framing layer entirely. Encryption, if enabled, still applies.
    /// </summary>
    /// <param name="bytes">The bytes to put on the wire as-is. Not retained after the call completes.</param>
    /// <param name="cancellationToken">A token to cancel the send.</param>
    /// <returns>A task that completes when the bytes have been handed to the transport.</returns>
    /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The transport is closed; await <see cref="Completion"/> for the root cause.</exception>
    /// <exception cref="OperationCanceledException">The send was canceled through <paramref name="cancellationToken"/>.</exception>
    public ValueTask SendRawBytesAsync(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return SendRawBytesCoreAsync(bytes, cancellationToken);
    }

    private async ValueTask SendRawBytesCoreAsync(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
    {
        await _sendGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            FlushResult result = await _sendPipe.Writer.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
            ThrowIfSendCompleted(result);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw NewTransportClosedException();
        }
        finally
        {
            _sendGate.Release();
        }
    }

    // One closed-transport error for all three send doors — identical text, identical type.
    private static void ThrowIfSendCompleted(FlushResult result)
    {
        if (result.IsCompleted)
            throw NewTransportClosedException();
    }

    private static InvalidOperationException NewTransportClosedException() =>
        new("The transport is closed; await Completion for the root cause.");

    private static int WriteVarInt(Span<byte> destination, int value)
    {
        uint remaining = (uint)value;
        int written = 0;
        while (remaining >= 0x80)
        {
            destination[written++] = (byte)(remaining | 0x80);
            remaining >>= 7;
        }
        destination[written++] = (byte)remaining;
        return written;
    }

    // ------------------------------------------------------------------ pumps

    // Plain async methods with try/finally — the audit's core lesson. No ContinueWith,
    // no Task<Task> traps; both pump outcomes are collected below, honestly.
    private async Task RunPumpsAsync()
    {
        Task fill = FillReceivePipeAsync(_lifetimeCts.Token);
        Task drain = DrainSendPipeAsync(_lifetimeCts.Token);

        Task first = await Task.WhenAny(fill, drain).ConfigureAwait(false);
        if (first.IsFaulted)
        {
            // One pump died: bring the peer down through cancellation — never by
            // completing the peer's pipe ends from the outside (audit Г1).
            _lifetimeCts.Cancel();
        }

        try
        {
            await Task.WhenAll(fill, drain).ConfigureAwait(false);
        }
        catch
        {
            // Collected below from both tasks, not just the first.
        }

        List<Exception>? errors = null;
        Collect(fill, ref errors);
        Collect(drain, ref errors);
        if (errors is { Count: > 0 })
            _completion.TrySetException(errors);
        else
            _completion.TrySetResult();
    }

    private void Collect(Task pump, ref List<Exception>? errors)
    {
        if (!pump.IsFaulted)
            return;
        foreach (Exception ex in pump.Exception!.InnerExceptions)
        {
            if (ex is OperationCanceledException)
                continue; // cancellation is the shutdown signal, not a failure
            if (Volatile.Read(ref _disposeState) != 0 && ex is ObjectDisposedException or IOException)
                continue; // the stream was torn down by Dispose on purpose
            (errors ??= new List<Exception>()).Add(ex);
        }
    }

    private async Task FillReceivePipeAsync(CancellationToken cancellationToken)
    {
        PipeWriter output = _receivePipe.Writer;
        Exception? error = null;
        try
        {
            while (true)
            {
                Memory<byte> memory = output.GetMemory(ReadBufferHint);
                int bytesRead = await _stream.ReadAsync(memory, cancellationToken).ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    // Clean FIN. Completing the pipe without an error lets the frame
                    // layer end the enumeration softly at a frame boundary — and raise a
                    // truncation error if the cut landed mid-frame (audit Г2).
                    break;
                }

                // Cipher boundary, fill side: sample once per iteration and decrypt the
                // whole arrived chunk in place with the cipher active at arrival time.
                // The receive pipe therefore only ever holds plaintext. In place, no
                // wrapper pipe, no second copy (hardening plan Ш1, variant А).
                PacketCipher? decryptor = _decryptor;
                decryptor?.Transform(memory.Span[..bytesRead]);

                output.Advance(bytesRead);
                FlushResult flushed = await output.FlushAsync(cancellationToken).ConfigureAwait(false);
                if (flushed.IsCompleted || flushed.IsCanceled)
                    break; // the reading side is gone — shutdown
            }
        }
        catch (Exception ex)
        {
            error = ex;
            throw;
        }
        finally
        {
            // This pump completes only its OWN pipe end (audit Г1).
            await output.CompleteAsync(error).ConfigureAwait(false);
        }
    }

    private async Task DrainSendPipeAsync(CancellationToken cancellationToken)
    {
        PipeReader input = _sendPipe.Reader;
        Exception? error = null;
        try
        {
            while (true)
            {
                ReadResult result = await input.ReadAsync(cancellationToken).ConfigureAwait(false);
                ReadOnlySequence<byte> buffer = result.Buffer;
                if (!buffer.IsEmpty)
                {
                    // Cipher boundary, drain side: _encryptor changes only between
                    // iterations (TryPromotePendingEncryptor below), never mid-buffer.
                    if (_encryptor is null)
                    {
                        foreach (ReadOnlyMemory<byte> segment in buffer)
                            await _stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await WriteEncryptedAsync(buffer, _encryptor, cancellationToken).ConfigureAwait(false);
                    }
                    await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
                input.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                    break;

                // result.IsCanceled is not a stop signal here: CancelPendingRead is the
                // nudge EnableEncryption uses to run this check on a parked pump.
                TryPromotePendingEncryptor(input);
            }
        }
        catch (Exception ex)
        {
            error = ex;
            throw;
        }
        finally
        {
            // Wake a barrier waiter that would otherwise wait forever on a dead pump.
            _encryptorInstalled?.TrySetResult();
            // This pump completes only its OWN pipe end (audit Г1).
            await input.CompleteAsync(error).ConfigureAwait(false);
        }
    }

    private void TryPromotePendingEncryptor(PipeReader input)
    {
        if (Volatile.Read(ref _pendingEncryptor) is null)
            return;
        if (input.TryRead(out ReadResult queued))
        {
            // Bytes queued before the barrier are still in flight — they must leave as
            // plaintext. Put them back untouched; the next loop iteration drains them,
            // and the check runs again.
            input.AdvanceTo(queued.Buffer.Start, queued.Buffer.Start);
            return;
        }
        // The send pipe is verifiably empty and this is the pump's own execution path:
        // the only safe point to swap the cipher.
        PacketCipher? pending = Interlocked.Exchange(ref _pendingEncryptor, null);
        if (pending is not null)
        {
            _encryptor = pending;
            _encryptorInstalled?.TrySetResult();
        }
    }

    private async ValueTask WriteEncryptedAsync(
        ReadOnlySequence<byte> buffer, PacketCipher encryptor, CancellationToken cancellationToken)
    {
        // The pipe's buffer is read-only for us — encrypt through a rented scratch copy.
        // (The fill side has no such copy: it owns its writable chunk and transforms in place.)
        int scratchSize = (int)Math.Min(buffer.Length, EncryptChunkSize);
        byte[] scratch = ArrayPool<byte>.Shared.Rent(scratchSize);
        try
        {
            foreach (ReadOnlyMemory<byte> segment in buffer)
            {
                ReadOnlyMemory<byte> remaining = segment;
                while (!remaining.IsEmpty)
                {
                    int count = Math.Min(remaining.Length, scratch.Length);
                    remaining.Span[..count].CopyTo(scratch);
                    encryptor.Transform(scratch.AsSpan(0, count));
                    await _stream.WriteAsync(scratch.AsMemory(0, count), cancellationToken).ConfigureAwait(false);
                    remaining = remaining[count..];
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(scratch);
        }
    }

    // ------------------------------------------------------------------ disposal

    /// <summary>
    /// Stops the pumps, closes the transport and releases all resources.
    /// </summary>
    /// <returns>A task that completes when both pumps have stopped.</returns>
    /// <remarks>
    /// Never hangs: cancellation is followed by closing the stream, which forces the
    /// pumps out of blocking I/O even if the stream ignores cancellation (audit Г3).
    /// Idempotent; never throws transport errors — those live in <see cref="Completion"/>.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposeState, 1) != 0)
            return;
        ShutdownCore();
        await _pumps.ConfigureAwait(false); // the supervisor never faults; pump errors live in Completion
        _ = _completion.Task.Exception;     // observe a faulted Completion nobody awaited
        CleanupCore();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Synchronous counterpart of <see cref="DisposeAsync"/>.
    /// </summary>
    /// <remarks>
    /// Initiates the same shutdown but does not wait for the pumps — Dispose must not
    /// block on the network. Prefer <see cref="DisposeAsync"/>.
    /// </remarks>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposeState, 1) != 0)
            return;
        ShutdownCore();
        _ = ObserveAndCleanupAsync();
        GC.SuppressFinalize(this);
    }

    private void ShutdownCore()
    {
        // Order matters (audit Г3): cancel first, then close the stream so pumps stuck
        // in Read/Write are forced out even when the stream ignores the token. Only
        // after that may anyone wait for Completion.
        _lifetimeCts.Cancel();
        try { _receivePipe.Reader.CancelPendingRead(); } // wake a consumer parked inside the frame layer
        catch (InvalidOperationException) { }            // tolerated: FrameReader may have completed its end
        if (!_leaveOpen)
            _stream.Dispose();
    }

    private async Task ObserveAndCleanupAsync()
    {
        // Fire-and-forget from the sync Dispose: no exception may escape into
        // UnobservedTaskException — cleanup here is best-effort by definition.
        try
        {
            await _pumps.ConfigureAwait(false);
            _ = _completion.Task.Exception; // observe a faulted Completion nobody awaited
            CleanupCore();
        }
        catch
        {
        }
    }

    private void CleanupCore()
    {
        // Runs strictly after both pumps have stopped. The consumer-facing pipe ends may
        // still be touched by a racing enumeration or send — the Complete calls are
        // best-effort to return pooled segments deterministically.
        try { _receivePipe.Reader.Complete(); }
        catch (InvalidOperationException) { }
        try { _sendPipe.Writer.Complete(); }
        catch (InvalidOperationException) { }

        _frameWriter.Dispose(); // Integrator note: frees the writer's cached compressor.
        _decryptor?.Dispose();
        _encryptor?.Dispose();
        Interlocked.Exchange(ref _pendingEncryptor, null)?.Dispose();
        _cipherOwner?.Dispose();

        // _sendGate and _lifetimeCts are deliberately NOT disposed. A straggling sender
        // can still be inside SendPacketCoreAsync: its finally would then Release a
        // disposed semaphore and mask the real send error with ObjectDisposedException.
        // A SemaphoreSlim whose AvailableWaitHandle was never touched and a CTS without
        // timers hold no unmanaged state, so skipping Dispose is safe (the runtime's own
        // practice for exactly this shutdown race).
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposeState) != 0, this);

    // ------------------------------------------------------------------ ciphers

    // The AES-128/CFB8 cipher pair lives in AesCfb8Cipher.cs (internal): PacketCipher over the
    // platform's Aes.EncryptCfb/DecryptCfb, keeping the CFB8 shift register itself (Д5).
}
