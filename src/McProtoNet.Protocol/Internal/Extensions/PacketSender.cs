using McProtoNet.Client;

namespace McProtoNet.Protocol;

public struct PacketSender<T> where T : IClientPacket, new()
{
    internal PacketSender(MinecraftClient client)
    {
        _client = client;
    }

    private MinecraftClient _client;
    public T Packet { get; set; } = new();

    public ValueTask Send()
    {
        //Check null
        if (Packet is null)
        {
            throw new ArgumentNullException(nameof(Packet));
        }

        return _client.SendPacket(Packet);
    }
}