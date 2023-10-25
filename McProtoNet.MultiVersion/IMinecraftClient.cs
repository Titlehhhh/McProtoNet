using McProtoNet.MultiVersion.Client;


namespace McProtoNet.MultiVersion
{
	public interface IMinecraftClient : IMinecraftClientActions, IMinecraftClientEvents, IDisposable, IAsyncDisposable
	{
		public ClientState State { get; }
		public event EventHandler<StateChangedEventArgs> StateChanged;


		Task Disconnect();
	}
}