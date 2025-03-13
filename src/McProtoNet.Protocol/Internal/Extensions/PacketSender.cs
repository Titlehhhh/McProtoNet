using McProtoNet.Abstractions;
using McProtoNet.Client;

namespace McProtoNet.Protocol;

public struct PacketSender<T> where T : IClientPacket, new()
{
    internal PacketSender(IMinecraftClient client)
    {
        _client = client;
    }

    private IMinecraftClient _client;
    public T Packet { get; set; } = new();

    public ValueTask Send(CancellationToken token = default)
    {
        //Check null
        if (Packet is null)
        {
            throw new ArgumentNullException(nameof(Packet));
        }

        return _client.SendPacket(Packet, token);
    }
}