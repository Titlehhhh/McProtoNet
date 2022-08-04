namespace McProtoNet.Protocol754.Data.World.Map
{
    public class MapData
    {
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public byte[] Data { get; private set; }

        public MapData(int columns, int rows, int x, int y, byte[] data)
        {
            Columns = columns;
            Rows = rows;
            X = x;
            Y = y;
            Data = data;
        }
    }
}
