// dotnet run [host] [port] [name] — по умолчанию 127.0.0.1 25565 McProtoBot (pv 772)

using McProtoNet;
using McProtoNet.Primitives;
using McProtoNet.Protocol;
using McProtoNet.Transport.Cryptography;
using MinimalBot.Packets;
using ConfCb = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using ConfSb = McProtoNet.Protocol.Packets.Configuration.Serverbound;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginCb = McProtoNet.Protocol.Packets.Login.Clientbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;
using PlayCb = McProtoNet.Protocol.Packets.Play.Clientbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;

const int Pv = 772;

var host = args.Length > 0 ? args[0] : "127.0.0.1";
var port = args.Length > 1 && int.TryParse(args[1], out var typed) ? typed : 25565;
var name = args.Length > 2 ? args[2] : "McProtoBot";

Console.WriteLine($"MinimalBot: {name} -> {host}:{port} (pv {Pv})");

await using var client = new MinecraftClient(new MinecraftClientOptions { Host = host, Port = port });
await client.ConnectAsync();
Console.WriteLine("[net] соединение открыто");

await client.SendAsync(new HandshakeSb.SetProtocolPacket(Pv, host, port, 2), Pv);
await client.SendAsync(new LoginSb.LoginStartPacket(name, V764_Last: new(Guid.NewGuid())), Pv);
Console.WriteLine("[login] handshake + login start отправлены");

var bot = new Bot(client, Pv);

await foreach (var packet in client.ReadPacketsAsync())
{
    await bot.HandleAsync(in packet, Pv);
    if (bot.Stopped) break;
}

Console.WriteLine("Соединение закрыто.");

sealed class Bot(MinecraftClient client, int pv) : ClientboundHandler
{
    private readonly HashSet<(PacketPhase, int)> _unknownSeen = [];
    private int _keepAlives;
    private bool _spawned;

    /// <summary>Сессия окончена: кик, ошибка входа или конец потока.</summary>
    public bool Stopped { get; private set; }

    protected override ValueTask OnLoginCompress(LoginCb.LoginCompressPacket packet)
    {
        client.CompressionThreshold = packet.Threshold;
        Console.WriteLine($"[login] компрессия включена, порог {packet.Threshold}");
        return default;
    }

    protected override async ValueTask OnEncryptionRequest(LoginCb.EncryptionRequestPacket packet)
    {
        if (packet.V766_Last is { ShouldAuthenticate: true })
        {
            Console.WriteLine("[login] сервер требует Mojang-авторизацию — пример работает только с offline");
            Stopped = true;
            return;
        }

        using var rsa = EncryptionHelpers.DecodeRSAPublicKey(packet.PublicKey)
                        ?? throw new InvalidOperationException("не разобрали открытый ключ сервера");
        var secret = EncryptionHelpers.GenerateAESPrivateKey();

        await client.SendAsync(new LoginSb.EncryptionResponsePacket(
            rsa.Encrypt(secret, false),
            rsa.Encrypt(packet.VerifyToken, false)), pv);

        client.EnableEncryption(secret);
        Console.WriteLine("[login] шифр включён: AES/CFB8");
    }

    protected override ValueTask OnLoginPluginRequest(LoginCb.LoginPluginRequestPacket packet)
        => client.SendAsync(new LoginSb.LoginPluginResponsePacket(packet.MessageId, null), pv);

    protected override async ValueTask OnLoginSuccess(LoginCb.LoginSuccessPacket packet)
    {
        Console.WriteLine($"[login] успех: {packet.Username} {packet.Uuid}");
        await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), pv);
        Phase = PacketPhase.Configuration;

        await client.SendAsync(new ConfSb.ClientInformationPacket(
            "en_us", 2, 0, true, 0x7F, 1, false, true, V768_Last: new(0)), pv);
        Console.WriteLine("[config] вошли в configuration, client information отправлена");
    }

    protected override ValueTask OnLoginDisconnect(LoginCb.LoginDisconnectPacket packet)
        => Kicked("login", packet.Reason);

    protected override ValueTask OnConfigurationKeepAlive(ConfCb.KeepAlivePacket packet)
        => client.SendAsync(new ConfSb.KeepAlivePacket(packet.KeepAliveId), pv);

    protected override ValueTask OnConfigurationPing(ConfCb.PingPacket packet)
        => client.SendAsync(new ConfSb.PongPacket(packet.Id), pv);

    protected override async ValueTask OnSelectKnownPacks(ConfCb.SelectKnownPacksPacket packet)
    {
        await client.SendAsync(new ConfSb.SelectKnownPacksPacket(packet.Packs), pv);
        Console.WriteLine($"[config] known packs: {packet.Packs.Length} шт, подтвердили");
    }

    protected override async ValueTask OnFinishConfiguration(ConfCb.FinishConfigurationPacket packet)
    {
        await client.SendAsync(new ConfSb.FinishConfigurationPacket(), pv);
        Phase = PacketPhase.Play;
        Console.WriteLine("[play] configuration завершена, вошли в play");
    }

    protected override ValueTask OnDisconnect(ConfCb.DisconnectPacket packet)
        => Kicked("config", packet.V764?.ReasonJson ?? packet.V765_Last?.Reason.ToString());

    protected override async ValueTask OnKeepAlive(PlayCb.KeepAlivePacket packet)
    {
        await client.SendAsync(new PlaySb.KeepAlivePacket(packet.KeepAliveId), pv);
        await SendLocalAsync(ArmAnimationPacket.GetPacketId(pv), new ArmAnimationPacket(Hand: 0));
        Console.WriteLine($"[play] keep-alive #{++_keepAlives} — ответили и махнули рукой");
    }

    protected override ValueTask OnPing(PlayCb.PingPacket packet)
        => client.SendAsync(new PlaySb.PongPacket(packet.Id), pv);

    protected override async ValueTask OnStartConfiguration(PlayCb.StartConfigurationPacket packet)
    {
        await client.SendAsync(new PlaySb.ConfigurationAcknowledgedPacket(), pv);
        Phase = PacketPhase.Configuration;
        Console.WriteLine("[config] сервер вернул нас в configuration");
    }

    protected override async ValueTask OnPlayerPosition(PlayCb.PlayerPositionPacket packet)
    {
        await client.SendAsync(new PlaySb.TeleportConfirmPacket(packet.TeleportId), pv);
        Console.WriteLine(
            $"[play] позиция: x={packet.X:F1} y={packet.Y:F1} z={packet.Z:F1} — телепорт {packet.TeleportId} подтверждён");

        if (!_spawned)
        {
            _spawned = true;
            Console.WriteLine("[play] спавн");
        }
    }

    protected override ValueTask OnUpdateHealth(PlayCb.UpdateHealthPacket packet)
    {
        Console.WriteLine($"[play] здоровье: {packet.Health}, еда {packet.Food}");
        return default;
    }

    protected override ValueTask OnKickDisconnect(PlayCb.KickDisconnectPacket packet)
        => Kicked("play", packet.VUntil764?.ReasonJson ?? packet.V765_Last?.Reason.ToString());

    protected override ValueTask OnUnknown(in IncomingPacket raw)
    {
        if (_unknownSeen.Add((Phase, raw.Id)))
        {
            var packetName = PacketRegistry.TryResolve(raw.Id, pv, Phase, Direction, out var desc)
                ? desc.Identity.Name
                : $"0x{raw.Id:X2}";
            Console.WriteLine($"  .. [{Phase}] пропущен {packetName} ({raw.Body.Length} байт) — и все следующие такие");
        }

        return default;
    }

    private ValueTask Kicked(string phase, string? reason)
    {
        Console.WriteLine($"[{phase}] кик: {reason ?? "без причины"}");
        Stopped = true;
        return default;
    }

    private async ValueTask SendLocalAsync<T>(int id, T packet) where T : IProtocolType<T>
    {
        var writer = MinecraftPrimitiveWriterCache.Rent();
        try
        {
            packet.Write(writer, pv);
            await client.SendRawAsync(id, writer.WrittenMemory);
        }
        finally
        {
            MinecraftPrimitiveWriterCache.Return(writer);
        }
    }
}
