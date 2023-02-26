namespace McProtoNet
{
    internal class Dumper
    {
        internal static void Dump(string text)
        {
            using (StreamWriter sw = new StreamWriter("dump.txt", true))
                sw.WriteLine(text);
        }
        internal static void Dump(byte b)
        {
            using (StreamWriter sw = new StreamWriter("dump.txt", true))
                sw.WriteLine(b.ToString());
        }
        internal static void Dump(byte[] data)
        {
            foreach (var i in data)
                Dump(i);
        }
    }
}
