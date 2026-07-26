// MinimalBot — задача 7 (фаза 1 ADR-001): исполняемая документация ядра.
// Стиль "A + Decode": транспорт -> ручной свитч по id -> статические Read/Write
// сгенерированных пакетов. Никаких фасадов, только ядро.
//
// Использование: dotnet run [host] [port] [name]
// По умолчанию: 127.0.0.1 25566 McProtoBot (paper-1.21.8, pv 772)

using System.Net.Sockets;
using McProtoNet.Client;
using McProtoNet.Net;
using McProtoNet.Protocol;
using McProtoNet.Serialization;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginCb = McProtoNet.Protocol.Packets.Login.Clientbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;
using ConfCb = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using ConfSb = McProtoNet.Protocol.Packets.Configuration.Serverbound;
using PlayCb = McProtoNet.Protocol.Packets.Play.Clientbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;
using MinimalBot.Packets; // локальные пакеты — собраны прямо здесь, на ядре

const int Pv = 772; // 1.21.7/8

var host = args.Length > 0 ? args[0] : "127.0.0.1";
var port = args.Length > 1 ? int.Parse(args[1]) : 25566;
var name = args.Length > 2 ? args[2] : "McProtoBot";

Console.WriteLine($"MinimalBot: {name} -> {host}:{port} (pv {Pv})");

using var tcp = new TcpClient();
await tcp.ConnectAsync(host, port);
await using var client = PipelinesMinecraftClient.Create(tcp.GetStream(), Pv);

// --- отправка: varint id + тело, транспорт сам допишет длину и компрессию ---
async ValueTask SendAsync<T>(int id, T packet) where T : IProtocolType<T>
{
    var writer = new MinecraftPrimitiveWriter();
    writer.WriteVarInt(id);
    packet.Write(writer, Pv);
    using var owner = writer.GetWrittenMemory();
    await client.SendPacketAsync(owner.Memory);
}

// --- чат: подпись не шлём (offline), сервер ограничивает сообщение 256 символами ---
async ValueTask SendChatAsync(string text)
{
    if (text.Length > 256) text = text[..256];
    await SendAsync(ChatMessagePacket.GetPacketId(Pv),
        new ChatMessagePacket(text, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Salt: 0));
}

// --- приём: Data валидна только до следующего пакета, декодим сразу ---
static T Decode<T>(in InputPacket packet) where T : IProtocolType<T>
{
    var reader = new MinecraftPrimitiveReader(packet.Data);
    var value = T.Read(ref reader, Pv);
    if (reader.RemainingCount != 0)
        Console.WriteLine($"  !! {typeof(T).Name}: не дочитано {reader.RemainingCount} байт — ошибка спеки?");
    return value;
}

// handshake: intent=2 (login), дальше сразу login start
await SendAsync(HandshakeSb.SetProtocolPacket.GetPacketId(Pv),
    new HandshakeSb.SetProtocolPacket(Pv, host, port, 2));
await SendAsync(LoginSb.LoginStartPacket.GetPacketId(Pv),
    new LoginSb.LoginStartPacket(name, null, Guid.NewGuid()));
Console.WriteLine("[login] handshake + login start отправлены");

var phase = Phase.Login;
var unknownSeen = new HashSet<(Phase, int)>();
var startedAt = DateTime.UtcNow; // для статуса «живу N сек»
long totalPackets = 0;           // все принятые пакеты, любых фаз
var playKeepAlives = 0;          // каждый 4-й — статус в чат (~раз в минуту)
var greeted = false;             // приветствие один раз, после первого спавна

await foreach (var packet in client.ReadPacketsAsync())
{
    totalPackets++;
    switch (phase)
    {
        case Phase.Login:
            if (packet.Id == LoginCb.LoginCompressPacket.GetPacketId(Pv))
            {
                var compress = Decode<LoginCb.LoginCompressPacket>(packet);
                client.CompressionThreshold = compress.Threshold;
                Console.WriteLine($"[login] компрессия включена, порог {compress.Threshold}");
            }
            else if (packet.Id == LoginCb.LoginSuccessPacket.GetPacketId(Pv))
            {
                var success = Decode<LoginCb.LoginSuccessPacket>(packet);
                Console.WriteLine($"[login] успех: {success.Username} {success.Uuid}");
                await SendAsync(LoginSb.LoginAcknowledgedPacket.GetPacketId(Pv),
                    new LoginSb.LoginAcknowledgedPacket());
                phase = Phase.Configuration;
                await SendAsync(ConfSb.ClientInformationPacket.GetPacketId(Pv),
                    new ConfSb.ClientInformationPacket("en_us", 8, 0, true, 0x7F, 1, false, true, 0));
                Console.WriteLine("[config] вошли в configuration, client information отправлена");
            }
            else if (packet.Id == LoginCb.LoginDisconnectPacket.GetPacketId(Pv))
            {
                Console.WriteLine($"[login] кик: {Decode<LoginCb.LoginDisconnectPacket>(packet).Reason}");
                return;
            }
            else LogUnknown(phase, packet);
            break;

        case Phase.Configuration:
            if (packet.Id == ConfCb.KeepAlivePacket.GetPacketId(Pv))
            {
                var keepAlive = Decode<ConfCb.KeepAlivePacket>(packet);
                await SendAsync(ConfSb.KeepAlivePacket.GetPacketId(Pv),
                    new ConfSb.KeepAlivePacket(keepAlive.KeepAliveId));
            }
            else if (packet.Id == ConfCb.PingPacket.GetPacketId(Pv))
            {
                var ping = Decode<ConfCb.PingPacket>(packet);
                await SendAsync(ConfSb.PongPacket.GetPacketId(Pv), new ConfSb.PongPacket(ping.Id));
            }
            else if (packet.Id == ConfCb.SelectKnownPacksPacket.GetPacketId(Pv))
            {
                var packs = Decode<ConfCb.SelectKnownPacksPacket>(packet);
                await SendAsync(ConfSb.SelectKnownPacksPacket.GetPacketId(Pv),
                    new ConfSb.SelectKnownPacksPacket(packs.Packs));
                Console.WriteLine($"[config] known packs: {packs.Packs.Length} шт, подтвердили");
            }
            else if (packet.Id == ConfCb.FinishConfigurationPacket.GetPacketId(Pv))
            {
                await SendAsync(ConfSb.FinishConfigurationPacket.GetPacketId(Pv),
                    new ConfSb.FinishConfigurationPacket());
                phase = Phase.Play;
                Console.WriteLine("[play] configuration завершена, вошли в play");
            }
            else if (packet.Id == ConfCb.DisconnectPacket.GetPacketId(Pv))
            {
                Console.WriteLine($"[config] кик: {Decode<ConfCb.DisconnectPacket>(packet).Reason}");
                return;
            }
            else LogUnknown(phase, packet);
            break;

        case Phase.Play:
            if (packet.Id == PlayCb.KeepAlivePacket.GetPacketId(Pv))
            {
                var keepAlive = Decode<PlayCb.KeepAlivePacket>(packet);
                await SendAsync(PlaySb.KeepAlivePacket.GetPacketId(Pv),
                    new PlaySb.KeepAlivePacket(keepAlive.KeepAliveId));
                await SendAsync(ArmAnimationPacket.GetPacketId(Pv), new ArmAnimationPacket(Hand: 0));
                Console.WriteLine($"[play] keep-alive {keepAlive.KeepAliveId} — ответили и махнули рукой");
                if (++playKeepAlives % 4 == 0) // ~раз в минуту
                {
                    var aliveSec = (int)(DateTime.UtcNow - startedAt).TotalSeconds;
                    await SendChatAsync($"живу {aliveSec} сек, принял {totalPackets} пакетов ({unknownSeen.Count} типов незнакомых)");
                }
            }
            else if (packet.Id == PlayCb.PlayerPositionPacket.GetPacketId(Pv))
            {
                var pos = Decode<PlayCb.PlayerPositionPacket>(packet);
                await SendAsync(PlaySb.TeleportConfirmPacket.GetPacketId(Pv),
                    new PlaySb.TeleportConfirmPacket(pos.TeleportId));
                Console.WriteLine($"[play] позиция: x={pos.X:F1} y={pos.Y:F1} z={pos.Z:F1} — телепорт {pos.TeleportId} подтверждён");
                if (!greeted) // первый PlayerPosition = спавн
                {
                    greeted = true;
                    await SendChatAsync("Привет! Я MinimalBot на McProtoNet");
                    await SendChatAsync("пришёл по ядру: транспорт → свитч по id → статические Read/Write. Без фасадов, без рефлексии");
                    Console.WriteLine("[play] спавн — поздоровались в чате");
                }
            }
            else if (packet.Id == PlayCb.UpdateHealthPacket.GetPacketId(Pv))
            {
                var health = Decode<PlayCb.UpdateHealthPacket>(packet);
                Console.WriteLine($"[play] здоровье: {health.Health}, еда {health.Food}");
            }
            else LogUnknown(phase, packet);
            break;
    }
}

Console.WriteLine("Соединение закрыто сервером.");

void LogUnknown(Phase p, in InputPacket packet)
{
    if (unknownSeen.Add((p, packet.Id)))
        Console.WriteLine($"  .. [{p}] незнакомый пакет 0x{packet.Id:X2} ({packet.Data.Length} байт) — пропускаем (и все следующие такие)");
}

enum Phase { Login, Configuration, Play }
