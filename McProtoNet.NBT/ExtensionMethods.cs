using System.Text;

namespace McProtoNet.NBT
{
    public static class ExtensionMethods
    {
        public static byte[] ToBytes(this string str, Encoding enc)
        {
            return enc.GetBytes(str);

        }

        public static byte[] ToBytes(this string str)
        {
            return ToBytes(str, Encoding.UTF8);
        }
    }
}
