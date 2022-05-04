namespace ProtoLib.API.Protocol
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketInfoAttribute : Attribute
    {
        public int ID { get; private set; }
        public int TargetVersion { get; private set; }
        public PacketCategory Category { get; private set; }
        public PacketSide Side { get; private set; }

        public PacketInfoAttribute(int iD, int targetVersion, PacketCategory category, PacketSide side)
        {
            ID = iD;
            TargetVersion = targetVersion;
            Category = category;
            Side = side;
        }
    }
}
