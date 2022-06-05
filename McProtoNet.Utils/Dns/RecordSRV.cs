namespace McProtoNet.Utils.Dns
{
    public class RecordSRV
    {
        public ushort PRIORITY;
        public ushort WEIGHT;
        public ushort PORT;
        public string TARGET;

        public RecordSRV(RecordReader rr)
        {
            PRIORITY = rr.ReadUInt16();
            WEIGHT = rr.ReadUInt16();
            PORT = rr.ReadUInt16();
            TARGET = rr.ReadDomainName();
        }
    }
}
