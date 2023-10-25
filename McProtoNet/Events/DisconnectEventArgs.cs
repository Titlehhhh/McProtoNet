namespace McProtoNet.Events
{
	public class DisconnectEventArgs : EventArgs
	{
		public string Reason { get; }

		public DisconnectEventArgs(string reason)
		{
			Reason = reason;
		}
	}
}
