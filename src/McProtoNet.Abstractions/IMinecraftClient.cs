namespace McProtoNet.Abstractions;

public interface IMinecraftClient
{
    ValueTask SendPacket(ReadOnlyMemory<byte> data);

    event PacketHandler PacketReceived;
    IObservable<InputPacket> OnPacket { get; }

    event EventHandler<StateEventArgs> StateChanged;

    event Action Disposed;

    Task Stop(Exception? customException = null);

    int ProtocolVersion { get; }
}