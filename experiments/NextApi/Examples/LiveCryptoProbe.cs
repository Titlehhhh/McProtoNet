// McProtoNet.Next experiment — examples: proof that the new AES/CFB8 path is byte-compatible
// with a REAL server, without a Mojang account.
//
// Run: dotnet run --project McProtoNet/experiments/NextApi -- login-crypto [host] [port] [name] [pv]
//
// The self-test (Demo/SelfTest.cs, checks (c), (d), (h)) only proves the cipher agrees with
// itself and with a one-shot oracle. Agreeing with the SERVER is a different claim, and the
// cheapest honest way to make it is an unauthenticated login: answer the encryption request
// with a random shared secret, switch the transport to ciphertext, and read what comes back.
// The server refuses the login — but the refusal arrives ENCRYPTED, so any decodable packet
// after the barrier means our ciphertext was accepted and our decryption is correct. No
// account, no session server, one round trip.
//
// The probe reads the RAW floor: when a cipher is wrong the bytes themselves are the only
// evidence worth printing, and the typed door does not hand them out.
//
// The probe makes exactly ONE distinction — the cipher agrees with the server, or it does
// not. Everything that stops the run before that question can be asked (no server on the
// port, a socket that dies during login, a refusal that arrives before the encryption
// request) is reported as INCONCLUSIVE, never as a cipher failure: a verdict the operator
// acts on must not be produced by a mistyped port.

using System.Buffers;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using McProtoNet.Net;
using McProtoNet.Protocol;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginCb = McProtoNet.Protocol.Packets.Login.Clientbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;

namespace McProtoNet.Next.Examples;

/// <summary>
///     Drives login up to the encryption barrier against a live server and reports whether
///     anything decodable arrives afterwards. Returns a process exit code: 0 = cipher proven,
///     1 = cipher failure, 2 = the server is in offline mode and never asked for encryption,
///     3 = inconclusive (the run never reached the cipher question).
/// </summary>
internal static class LiveCryptoProbe
{
    // Same handshake "next state" the bot uses for a login connection.
    private const int LoginIntent = 2;

    // The protocol's own secret size: AES-128, the secret doubling as the IV.
    private const int SharedSecretLength = 16;

    // Bytes of the first post-cipher body kept for the failure dump.
    private const int HeadDumpLength = 32;

    // Exit codes; also the switch of the verdict printer.
    private const int ExitProven = 0;
    private const int ExitFailed = 1;
    private const int ExitSkipped = 2;
    private const int ExitUnclear = 3;

    private static readonly TimeSpan ProbeCap = TimeSpan.FromSeconds(45);

    /// <summary>Runs the probe under a hard time cap and prints the verdict.</summary>
    public static async Task<int> RunAsync(string host, int port, string name, int protocolVersion)
    {
        using var cts = new CancellationTokenSource(ProbeCap);
        var state = new ProbeState();
        MinecraftClient? client = null;
        Verdict verdict;

        try
        {
            Step($"connecting to {host}:{port}, protocol {protocolVersion}");
            client = await MinecraftClient.ConnectAsync(host, port, cancellationToken: cts.Token)
                .ConfigureAwait(false);
            Step("connected");
            verdict = await ProbeAsync(client, host, port, name, protocolVersion, state, cts.Token)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            verdict = state.Encrypted
                ? Failed($"nothing arrived within {ProbeCap.TotalSeconds:F0} s after the cipher went live")
                : Unclear($"no verdict within {ProbeCap.TotalSeconds:F0} s, and the server never sent an "
                    + "encryption request — the cipher was never exercised");
        }
        catch (Exception ex) when (!state.Encrypted && IsEnvironment(ex))
        {
            // A server that is not there, a wrong port, a socket that dies during plaintext
            // login: the run stopped before the question. Saying CIPHER FAIL here would
            // collapse the one distinction this probe exists to make.
            verdict = Unclear($"the connection failed before any encryption request — "
                + $"{ex.GetType().Name}: {ex.Message}");
        }
        catch (Exception ex)
        {
            verdict = Failed(state.Encrypted
                ? $"the connection broke after the cipher went live — {ex.GetType().Name}: {ex.Message}"
                : $"login failed before the cipher — {ex.GetType().Name}: {ex.Message}");
        }
        finally
        {
            // Disposal is what stops the pumps; only after it is Completion final.
            if (client is not null)
                await client.DisposeAsync().ConfigureAwait(false);
        }

        return Print(verdict, state, TransportRootCause(client));
    }

    private static async Task<Verdict> ProbeAsync(
        MinecraftClient client, string host, int port, string name, int protocolVersion,
        ProbeState state, CancellationToken cancellationToken)
    {
        await client.SendAsync(
            new HandshakeSb.SetProtocolPacket(protocolVersion, host, port, LoginIntent), protocolVersion, cancellationToken);
        await client.SendAsync(
            new LoginSb.LoginStartPacket(name, V764_Last: new(Guid.NewGuid())), protocolVersion, cancellationToken);
        Step($"handshake (next state {LoginIntent}) + login start sent as {name}");

        await foreach (InputPacket raw in client.ReadPacketsAsync(cancellationToken))
        {
            if (state.Encrypted && state.Head is null)
            {
                // The head of the FIRST post-cipher frame. Copied before decoding, because a
                // decoder that throws leaves these bytes as the only evidence of what the
                // cipher produced — and the window dies on the next iteration.
                state.Head = raw.Data.Slice(0, Math.Min(HeadDumpLength, raw.Data.Length)).ToArray();
                state.HeadId = raw.Id;
            }

            // A body that does not decode is a verdict here, not an exception: the Try door
            // hands back the reason and the probe turns it into the verdict line directly.
            if (!PacketFlow.TryDecode(in raw, protocolVersion, PacketPhase.Login, PacketDirection.Clientbound,
                    out IPacket? packet, out DecodeError error))
                return state.Encrypted
                    ? Failed($"the first packet after the cipher did not decode — {error}")
                    : Failed($"login packet 0x{raw.Id:X2} did not decode — {error}");

            switch (packet)
            {
                case LoginCb.EncryptionRequestPacket request when !state.Encrypted:
                    Step($"encryption request: serverId='{request.ServerId}', publicKey {request.PublicKey.Length} B, "
                        + $"verifyToken {request.VerifyToken.Length} B, shouldAuthenticate={request.V766_Last?.ShouldAuthenticate}");
                    await RespondAsync(client, request, protocolVersion, state, cancellationToken);
                    break;

                case LoginCb.LoginCompressPacket compress when !state.Encrypted:
                    // What was OBSERVED: no encryption request before Set Compression. Vanilla
                    // and Paper only compress after authentication, so this means offline mode
                    // — but the order is a server behaviour, not a protocol guarantee.
                    return Skipped("no encryption request arrived before Set Compression "
                        + $"(threshold {compress.Threshold}) — the server is not asking this client to authenticate");

                case LoginCb.LoginSuccessPacket successPlain when !state.Encrypted:
                    return Skipped("no encryption request arrived before Login Success "
                        + $"({successPlain.Username} was logged straight in) — the server is in offline mode");

                case LoginCb.LoginDisconnectPacket kickPlain when !state.Encrypted:
                    // A refusal before the encryption request is a server policy answer
                    // (whitelist, full, wrong version), not a statement about the cipher.
                    return Unclear($"the server refused the login before any encryption request — {kickPlain.Reason}");

                case LoginCb.LoginCompressPacket compress:
                    // The cipher is already proven by this packet arriving at all; keep reading,
                    // because the refusal text is the more informative outcome for the operator.
                    client.CompressionThreshold = compress.Threshold;
                    state.Proof ??= $"LoginCompress threshold={compress.Threshold}";
                    Step($"[encrypted] set compression {compress.Threshold} — reading on for the refusal");
                    break;

                case LoginCb.LoginSuccessPacket success:
                    return Proven($"LoginSuccess {success.Username} {success.Uuid}");

                case LoginCb.LoginDisconnectPacket kick:
                    return Proven($"LoginDisconnect {kick.Reason}");

                case UnknownPacket unmapped when state.Encrypted:
                    // A mapped id would prove the cipher; an unmapped one means the decrypted
                    // bytes are not a login packet at all — that is what garbage looks like.
                    return Failed(
                        $"the first packet after the cipher carries id 0x{unmapped.Id:X2}, which the registry "
                        + "does not map in the Login phase — the decrypted bytes are not a login packet");

                case UnknownPacket unmappedPlain:
                    Step($"login id 0x{unmappedPlain.Id:X2} has no mapping — skipped");
                    break;

                default:
                    // LoginPluginRequest and LoginCookieRequest are mapped packets: after the
                    // barrier their arrival is itself the proof.
                    if (state.Encrypted)
                        state.Proof ??= packet.GetType().Name;
                    Step($"[{(state.Encrypted ? "encrypted" : "plain")}] {packet.GetType().Name}");
                    break;
            }
        }

        if (state.Proof is not null)
            return Proven($"{state.Proof}, then the server closed the connection");

        return state.Encrypted
            ? Failed("the server closed the connection without sending anything after the cipher")
            : Unclear("the server closed the connection before any encryption request — nothing was ciphered");
    }

    private static async Task RespondAsync(
        MinecraftClient client, LoginCb.EncryptionRequestPacket request, int protocolVersion,
        ProbeState state, CancellationToken cancellationToken)
    {
        byte[] sharedSecret = RandomNumberGenerator.GetBytes(SharedSecretLength);

        using var rsa = RSA.Create();
        // The key travels as X.509 SubjectPublicKeyInfo (DER) — the wire format, not a bare modulus.
        rsa.ImportSubjectPublicKeyInfo(request.PublicKey, out _);
        byte[] encryptedSecret = rsa.Encrypt(sharedSecret, RSAEncryptionPadding.Pkcs1);
        byte[] encryptedToken = rsa.Encrypt(request.VerifyToken, RSAEncryptionPadding.Pkcs1);
        Step($"shared secret {SharedSecretLength} B -> {encryptedSecret.Length} B, "
            + $"verify token -> {encryptedToken.Length} B (RSA/PKCS#1 v1.5, separately)");

        await client.SendAsync(
            new LoginSb.EncryptionResponsePacket(encryptedSecret, encryptedToken), protocolVersion, cancellationToken);
        Step("encryption response sent — the last plaintext frame");

        // The barrier: everything queued before this call leaves as plaintext, everything
        // after it is ciphertext. Awaiting it is what makes the next read a real test.
        await client.EnableEncryptionAsync(sharedSecret, cancellationToken);
        state.Encrypted = true;
        Step("cipher active in both directions; every byte from here is AES/CFB8");
    }

    // ------------------------------------------------------------------ verdict

    /// <summary>One outcome of the probe: the exit code and the line that explains it.</summary>
    private readonly record struct Verdict(int Code, string Line);

    private static Verdict Proven(string line) => new(ExitProven, line);
    private static Verdict Failed(string line) => new(ExitFailed, line);
    private static Verdict Skipped(string line) => new(ExitSkipped, line);
    private static Verdict Unclear(string line) => new(ExitUnclear, line);

    /// <summary>Everything the read loop learned that the outer handlers must also see.</summary>
    private sealed class ProbeState
    {
        public bool Encrypted;
        public byte[]? Head;
        public int HeadId;
        public string? Proof;
    }

    // Failures that say nothing about the cipher: no server, wrong port, a socket torn down.
    private static bool IsEnvironment(Exception ex) =>
        ex is SocketException or IOException or TimeoutException or ObjectDisposedException;

    // Read strictly AFTER DisposeAsync: only then have the pumps stopped and Completion is
    // final. DisposeAsync already observed a faulted Completion, so touching it here is safe.
    private static string? TransportRootCause(MinecraftClient? client)
    {
        if (client?.Completion is not { IsFaulted: true } completion)
            return null;
        return string.Join("; ", completion.Exception!.Flatten().InnerExceptions
            .Select(inner => $"{inner.GetType().Name}: {inner.Message}"));
    }

    private static int Print(Verdict verdict, ProbeState state, string? transportRootCause)
    {
        Console.WriteLine(verdict.Code switch
        {
            ExitProven => $"CIPHER OK: {verdict.Line}",
            ExitSkipped => $"SKIPPED: {verdict.Line}",
            ExitUnclear => $"INCONCLUSIVE: {verdict.Line}",
            _ => $"CIPHER FAIL: {verdict.Line}",
        });

        // The byte dump belongs to the post-cipher failure only: before the barrier there is
        // no barrier to report bytes "after".
        if (verdict.Code == ExitFailed && state.Encrypted)
        {
            Console.WriteLine(state.Head is null
                ? "  first bytes: none — nothing decodable ever arrived after the barrier"
                : $"  first bytes: id 0x{state.HeadId:X2}, body {Hex(state.Head)}");
            Console.WriteLine(
                "  note: the transport arms the receive decryptor inside EnableEncryptionAsync, after "
                + "SendAsync has already queued the response — a server answer landing in that window "
                + "would be read as plaintext. An INTERMITTENT failure here is that client race, not the cipher.");
        }

        // The read loop surfaces a dead pump as a bare cancellation; the cause lives here.
        if (transportRootCause is not null && verdict.Code != ExitProven)
            Console.WriteLine($"  transport root cause: {transportRootCause}");

        return verdict.Code;
    }

    private static string Hex(ReadOnlySpan<byte> bytes)
    {
        var text = new StringBuilder(bytes.Length * 3);
        foreach (byte value in bytes)
        {
            if (text.Length > 0)
                text.Append(' ');
            text.Append(value.ToString("X2"));
        }

        return text.ToString();
    }

    // Every step is printed as it happens: when the probe stops, the last line says where.
    private static void Step(string message) => Console.WriteLine($"[crypto] {message}");
}
