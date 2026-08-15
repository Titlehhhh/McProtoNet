// FormationBots — рой ботов заходит на сервер и по команде встаёт в надпись.
//
// Использование: dotnet run [host] [port] [text] [auto-сек] [шаг]
// По умолчанию: 127.0.0.1 25565 McProtoNet (paper-latest, pv 772), шаг 1.0
// Команды: в консоли — form [текст] / line / scatter / quit;
//          в игре — просто напиши в чат !form [текст], !line или !scatter
// form строит буквы по одной; «form C#» сначала меняет текст надписи.
// Лишние боты (текст короче стартового) встают резервными шеренгами позади.
// auto-сек > 0 — самим встать в ряд и построиться (для смоука без рук)
// шаг — размер клетки в блоках; плотнее 0.7 — выключите столкновения:
//   /team add bots ; /team modify bots collisionRule never ; /team join bots @a
// Для массового входа на Paper: в bukkit.yml connection-throttle: -1

using System.Globalization;
using FormationBots;

const int Pv = 772;

// Предпросмотр надписи без сервера: dotnet run -- --render <текст>
if (args.Length > 0 && args[0] == "--render")
{
    var preview = args.Length > 1 ? string.Join(' ', args[1..]) : "McProtoNet";
    var unknownChars = Formation.Unknown(preview);
    if (unknownChars.Count > 0)
    {
        Console.WriteLine($"не знаю символы: {string.Join(" ", unknownChars)}");
        return;
    }

    var pts = Formation.Layout(preview);
    var width = pts.Max(p => p.Col) + 1;
    for (var row = 0; row < 7; row++)
    {
        var chars = new char[width];
        Array.Fill(chars, ' ');
        foreach (var p in pts.Where(p => p.Row == row)) chars[p.Col] = '#';
        Console.WriteLine(new string(chars));
    }

    Console.WriteLine($"{pts.Count} ботов, ширина {width}");
    return;
}

var host = args.Length > 0 ? args[0] : "127.0.0.1";
var port = args.Length > 1 ? int.Parse(args[1]) : 25565;
var text = args.Length > 2 ? args[2] : "McProtoNet";
var autoSeconds = args.Length > 3 ? int.Parse(args[3]) : 0;
var spacing = args.Length > 4 ? double.Parse(args[4], CultureInfo.InvariantCulture) : 1.0;
var prefix = args.Length > 5 ? args[5] : "Mc_"; // свой префикс имён для второй стаи рядом

var cells = Formation.Layout(text);
Console.WriteLine(
    $"«{text}»: {cells.Count} ботов, ширина {Formation.Width(text) * spacing:F0} блоков (шаг {spacing}) -> {host}:{port} (pv {Pv})");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var lastCommandAt = DateTime.MinValue;
var commandLock = new object();
var heardCount = 0;
var heardCommand = "";
var botsLock = new object();
CancellationTokenSource? walkCts = null;
List<(Bot Bot, double X, double Z)> lastMoves = [];
(double X, double Z)? anchor = null; // центр строя, замеряется один раз — повторный form не уползает

var bots = new List<Bot>(cells.Count);
var sessions = new List<Task>(cells.Count);

try
{
    for (var i = 0; i < cells.Count; i++)
        await StartBotAsync(i);

    await WaitSpawnsAsync();

    // одна волна добора вместо потерянных (кик, обрыв, троттлинг входов)
    var lostIndexes = Enumerable.Range(0, bots.Count).Where(i => !bots[i].Alive).ToList();
    if (lostIndexes.Count > 0)
    {
        Console.WriteLine($"добор: {lostIndexes.Count} ботов, вторая попытка через 3 с");
        await Task.Delay(3000, cts.Token);
        foreach (var i in lostIndexes)
            await StartBotAsync(i);
        await WaitSpawnsAsync();
    }
}
catch (OperationCanceledException)
{
    // Ctrl+C на старте: уходим на общий слив сессий
}

var inWorld = Snapshot().Count(b => b.Alive);
Console.WriteLine($"в мире {inWorld}/{cells.Count} ботов");
Console.WriteLine("команды: form [текст] | line | scatter | quit (в игре: напиши в чат !form C# и т.д.)");

if (autoSeconds > 0)
{
    _ = Task.Run(async () =>
    {
        await Task.Delay(TimeSpan.FromSeconds(autoSeconds), cts.Token);
        Console.WriteLine("[auto] в ряд");
        await Task.WhenAll(Execute("line"));
        await Task.Delay(3000, cts.Token);
        Console.WriteLine("[auto] строимся");
        var started = DateTime.UtcNow;
        await Task.WhenAll(Execute("form"));
        var placed = lastMoves.Where(m => m.Bot.Alive).ToList();
        var worst = placed
            .Select(m =>
            {
                var dx = m.Bot.Location.X - m.X;
                var dz = m.Bot.Location.Z - m.Z;
                return Math.Sqrt(dx * dx + dz * dz);
            })
            .DefaultIfEmpty(double.NaN)
            .Max();
        Console.WriteLine(
            $"[auto] форма собрана за {(DateTime.UtcNow - started).TotalSeconds:F0} с: " +
            $"{placed.Count} ботов, худшее отклонение {worst:F2} блока");
    }, cts.Token);
}

while (!cts.IsCancellationRequested)
{
    string? line;
    try
    {
        line = await Console.In.ReadLineAsync(cts.Token);
    }
    catch (OperationCanceledException)
    {
        break;
    }

    if (line is null)
    {
        // консоль закрыта — команды остаются доступны из игры через /say
        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
        }
        catch (OperationCanceledException)
        {
        }

        break;
    }

    var trimmed = line.Trim();
    if (trimmed.StartsWith("form", StringComparison.OrdinalIgnoreCase))
    {
        if (TrySetText(trimmed[4..].Trim()))
            Execute("form");
        continue;
    }

    switch (trimmed.ToLowerInvariant())
    {
        case "line":
            Execute("line");
            break;
        case "scatter":
            Execute("scatter");
            break;
        case "quit" or "q":
            cts.Cancel();
            break;
    }
}

foreach (var session in sessions)
{
    try
    {
        await session;
    }
    catch
    {
    }
}

Console.WriteLine("все сессии закрыты");
return;


async Task StartBotAsync(int index)
{
    var bot = new Bot($"{prefix}{index:D3}", host, port, Pv);
    bot.ChatHeard += OnChatHeard;
    lock (botsLock)
    {
        if (index < bots.Count) bots[index] = bot;
        else bots.Add(bot);
    }

    sessions.Add(RunSessionAsync(bot));
    await Task.Delay(100, cts.Token); // не душить сервер очередью входов
}

List<Bot> Snapshot()
{
    lock (botsLock)
    {
        return [.. bots];
    }
}

async Task RunSessionAsync(Bot bot)
{
    try
    {
        await bot.RunAsync(cts.Token);
    }
    catch (OperationCanceledException)
    {
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[{bot.Name}] обрыв: {ex.Message}");
    }

    if (bot.DisconnectReason is { } reason)
        Console.WriteLine($"[{bot.Name}] кик: {reason}");
}

async Task WaitSpawnsAsync()
{
    foreach (var bot in Snapshot().Where(b => b.Alive))
    {
        try
        {
            await bot.WaitForSpawnAsync(cts.Token).WaitAsync(TimeSpan.FromSeconds(60), cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[потеря] {ex.Message}");
        }
    }
}

bool TrySetText(string candidate)
{
    if (candidate.Length == 0) return true; // без аргумента — текущий текст
    var unknown = Formation.Unknown(candidate);
    if (unknown.Count > 0)
    {
        Console.WriteLine($"[текст] не знаю символы: {string.Join(" ", unknown)} — текст не сменился");
        return false;
    }

    if (candidate != text)
    {
        text = candidate;
        Console.WriteLine($"[текст] новая надпись: «{text}»");
    }

    return true;
}

void OnChatHeard(Bot bot, string message)
{
    try
    {
        var lower = message.ToLowerInvariant();
        string? command = null;
        var arg = "";
        var at = lower.IndexOf("!form", StringComparison.Ordinal);
        if (at >= 0)
        {
            command = "form";
            arg = message[(at + 5)..].Trim();
        }
        else if (lower.Contains("!line")) command = "line";
        else if (lower.Contains("!scatter")) command = "scatter";

        if (command is null) return;

        lock (commandLock)
        {
            // /say приходит каждому боту: первый исполняет, остальные считаются
            if (DateTime.UtcNow - lastCommandAt < TimeSpan.FromSeconds(2))
            {
                if (command == heardCommand) heardCount++;
                return;
            }

            lastCommandAt = DateTime.UtcNow;
            heardCommand = command;
            heardCount = 1;
        }

        var full = arg.Length > 0 ? $"!{command} {arg}" : $"!{command}";
        Console.WriteLine($"[чат] {bot.Name} услышал «{full}» — исполняю");
        _ = Task.Run(async () =>
        {
            await Task.Delay(1500, cts.Token);
            int heard;
            lock (commandLock)
            {
                heard = heardCount;
            }

            Console.WriteLine($"[чат] «{full}» услышали {heard} из {Snapshot().Count(b => b.Alive)} ботов");
        }, cts.Token);
        if (command == "form" && !TrySetText(arg)) return;
        Execute(command);
    }
    catch (Exception ex)
    {
        // ошибка обработчика не должна ронять сессию бота
        Console.WriteLine($"[чат] обработчик споткнулся: {ex.Message}");
    }
}

List<Task> Execute(string command)
{
    var alive = Snapshot().Where(b => b.Alive && b.Spawned).ToList();
    if (alive.Count == 0)
    {
        Console.WriteLine("живых ботов нет");
        return [];
    }

    CancellationTokenSource current;
    lock (commandLock)
    {
        var previous = walkCts;
        current = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
        walkCts = current;
        previous?.Cancel();
    }

    anchor ??= (alive.Average(b => b.Location.X), alive.Average(b => b.Location.Z));
    var (originX, originZ) = anchor.Value;

    List<(Bot Bot, double X, double Z)> moves = [];
    List<(double X, double Z)> targets = [];
    switch (command)
    {
        case "form":
        {
            // клетки пересчитываем: текст мог смениться командой «form <текст>»
            var points = Formation.Layout(text);
            var left = originX - Formation.Width(text) * spacing / 2.0;
            var top = originZ + 6;

            if (points.Count > alive.Count)
            {
                // ботов меньше, чем точек: прореживаем надпись равномерно, а не хвост
                var random = new Random(42);
                points = points.OrderBy(_ => random.Next()).Take(alive.Count).ToList();
            }

            var pool = new List<Bot>(alive);
            var letters = new List<List<(Bot Bot, double X, double Z)>>();
            foreach (var letterCells in points.GroupBy(c => c.Letter).OrderBy(g => g.Key))
            {
                var letterMoves = new List<(Bot Bot, double X, double Z)>();
                foreach (var cell in letterCells)
                {
                    var tx = left + cell.Col * spacing;
                    var tz = top + cell.Row * spacing;
                    var best = pool.MinBy(b =>
                    {
                        var dx = b.Location.X - tx;
                        var dz = b.Location.Z - tz;
                        return dx * dx + dz * dz;
                    })!;
                    pool.Remove(best);
                    letterMoves.Add((best, tx, tz));
                }

                letters.Add(letterMoves);
            }

            // лишние — резервными шеренгами позади надписи, чтобы не портили кадр
            var reserve = new List<(Bot Bot, double X, double Z)>();
            for (var i = 0; i < pool.Count; i++)
            {
                const int perRank = 40;
                var rank = i / perRank;
                var inRank = Math.Min(perRank, pool.Count - rank * perRank);
                reserve.Add((pool[i],
                    originX - (inRank - 1) * spacing / 2.0 + i % perRank * spacing,
                    top + 7 * spacing + 6 + rank * Math.Max(spacing, 1.0)));
            }

            lastMoves = letters.SelectMany(m => m).Concat(reserve).ToList();
            Console.WriteLine($"form «{text}»: {lastMoves.Count - reserve.Count} ботов в надписи"
                              + (reserve.Count > 0 ? $", {reserve.Count} в резерве" : "") + ", буквы по одной");
            return Track("form",
            [
                Task.Run(async () =>
                {
                    var reserveWalks = reserve.Select(m => Walk(m.Bot, m.X, m.Z, current.Token)).ToList();
                    foreach (var letter in letters)
                        await Task.WhenAll(letter.Select(m => Walk(m.Bot, m.X, m.Z, current.Token)));
                    await Task.WhenAll(reserveWalks);
                }, current.Token)
            ], current);
        }
        case "line":
        {
            // парадные шеренги перед надписью, максимум 40 ботов в шеренге
            const int perRank = 40;
            var rankGap = Math.Max(spacing, 1.0);
            for (var i = 0; i < alive.Count; i++)
            {
                var rank = i / perRank;
                var inRank = Math.Min(perRank, alive.Count - rank * perRank);
                var x = originX - (inRank - 1) * spacing / 2.0 + i % perRank * spacing;
                var z = originZ - 6 - rank * rankGap;
                targets.Add((x, z));
            }

            break;
        }
        default:
        {
            var random = new Random();
            moves.AddRange(alive.Select(bot =>
            {
                var angle = random.NextDouble() * Math.PI * 2;
                var radius = 6 + random.NextDouble() * 10;
                return (bot, originX + Math.Cos(angle) * radius, originZ + Math.Sin(angle) * radius);
            }));
            break;
        }
    }

    // ближайший свободный бот на каждую точку — меньше пересечений при ходьбе
    var free = new List<Bot>(alive);
    foreach (var target in targets)
    {
        var best = free.MinBy(b =>
        {
            var dx = b.Location.X - target.X;
            var dz = b.Location.Z - target.Z;
            return dx * dx + dz * dz;
        })!;
        free.Remove(best);
        moves.Add((best, target.X, target.Z));
    }

    lastMoves = moves;
    Console.WriteLine($"{command}: {moves.Count} ботов пошли");
    return Track(command, moves.Select(m => Walk(m.Bot, m.X, m.Z, current.Token)).ToList(), current);
}

List<Task> Track(string command, List<Task> walks, CancellationTokenSource current)
{
    var started = DateTime.UtcNow;
    _ = Task.Run(async () =>
    {
        try
        {
            await Task.WhenAll(walks);
        }
        catch
        {
        }

        if (!current.IsCancellationRequested)
            Console.WriteLine($"{command}: готово за {(DateTime.UtcNow - started).TotalSeconds:F0} с");
    });
    return walks;
}

Task Walk(Bot bot, double x, double z, CancellationToken token)
    => Task.Run(async () =>
    {
        try
        {
            await bot.WalkToAsync(x, z, token);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{bot.Name}] ходьба оборвалась: {ex.Message}");
        }
    }, token);
