NetworkStream ns = new NetworkStream(socket);
MinecraftPacketReader reader = new MinecraftPacketReader();
reader.BaseStream = ns;
CancellationTokenSource cts = new CancellationTokenSource();

while (!cts.IsCancellationRequested)
{
    InputPacket packet =
        await reader.ReadNextPacketAsync(cts.Token);
    Console.WriteLine(packet.Id);
}