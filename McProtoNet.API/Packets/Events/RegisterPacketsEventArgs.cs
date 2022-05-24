namespace McProtoNet.API.Packets
{
    public class RegisterPacketsEventArgs : EventArgs
    {
        public IList<Type> Packets { get; private set; }

        public RegisterPacketsEventArgs(IList<Type> packets)
        {
            Packets = packets;
        }
    }


}
