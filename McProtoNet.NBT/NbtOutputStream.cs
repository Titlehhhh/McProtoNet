using System.IO.Compression;

namespace McProtoNet.NBT
{
    public class NbtOutputStream
    {
        public Stream BaseStream { get; set; }

        public NbtOutputStream(Stream baseStream)
        {
            BaseStream = new GZipStream(baseStream, CompressionLevel.NoCompression);
        }

        public NbtOutputStream(Stream baseStream, bool gzip)
        {
            if (gzip)
            {
                BaseStream = new GZipStream(baseStream, CompressionLevel.Optimal);
            }

            BaseStream = baseStream;
        }

        public void WriteTag(NbtTag tag)
        {
            int type = (int)tag.TagType;
            string name = tag.Name;
            byte[] nameBytes = name.ToBytes();

            BaseStream.WriteByte((byte)type);
            BaseStream.Write( // Write as short
                new[]
                {
                    (byte) ((nameBytes.Length & 0xFF00) >> 8),
                    (byte) (nameBytes.Length & 0xFF)
                },
                0, 2);
            BaseStream.Write(nameBytes, 0, nameBytes.Length);

            if (tag.TagType == NbtTagType.End)
            {
                throw new Exception("Tag_End not permitted");
            }

            WriteTagPayload(tag);
        }

        private unsafe void WriteTagPayload(NbtTag tag)
        {
            switch (tag.TagType)
            {
                case NbtTagType.End:
                    break;
                case NbtTagType.Byte:
                    BaseStream.WriteByte(tag.ByteValue);
                    break;
                case NbtTagType.Short:
                    byte[] sBits = BitConverter.GetBytes(tag.ShortValue);
                    BaseStream.Write(sBits, 0, sBits.Length);
                    break;
                case NbtTagType.Int:
                    byte[] iBits = BitConverter.GetBytes(tag.IntValue);
                    BaseStream.Write(iBits, 0, iBits.Length);
                    break;
                case NbtTagType.Long:
                    byte[] lBits = BitConverter.GetBytes(tag.LongValue);
                    BaseStream.Write(lBits, 0, lBits.Length);
                    break;
                case NbtTagType.Float:
                    byte[] flBits = BitConverter.GetBytes(tag.FloatValue);
                    BaseStream.Write(flBits, 0, flBits.Length);
                    break;
                case NbtTagType.Double:
                    byte[] bits = BitConverter.GetBytes(tag.DoubleValue);
                    BaseStream.Write(bits, 0, bits.Length);
                    break;
                case NbtTagType.ByteArray:
                    byte[] bytes = tag.ByteArrayValue;
                    byte[] bitsLength = BitConverter.GetBytes(bytes.Length);

                    BaseStream.Write(bitsLength, 0, bitsLength.Length);
                    BaseStream.Write(bytes, 0, bytes.Length);
                    break;
                case NbtTagType.String:
                    byte[] bts = tag.StringValue.ToBytes();
                    byte[] btsLength = BitConverter.GetBytes(bts.Length);
                    BaseStream.Write(btsLength, 0, btsLength.Length);
                    BaseStream.Write(bts, 0, 0);
                    break;
                case NbtTagType.List:
                    NbtList li = (NbtList)tag;
                    BaseStream.WriteByte((byte)tag.TagType);
                    byte[] liCount = BitConverter.GetBytes(li.Count);
                    BaseStream.Write(liCount, 0, liCount.Length);

                    foreach (var t in li)
                    {
                        WriteTagPayload(t);
                    }
                    break;
                case NbtTagType.Compound:
                    foreach (NbtTag cTag in (NbtCompound)tag)
                    {
                        WriteTag(cTag);
                    }

                    BaseStream.WriteByte(0);
                    break;
                case NbtTagType.IntArray:
                    int[] ints = tag.IntArrayValue;
                    byte[] intsLength = BitConverter.GetBytes(ints.Length);
                    BaseStream.Write(intsLength, 0, intsLength.Length);

                    foreach (int i in ints)
                    {
                        byte[] bI = BitConverter.GetBytes(i);
                        BaseStream.Write(bI, 0, bI.Length);
                    }
                    break;
                default:
                    throw new Exception("Invalid tag type");
            }
        }

        public void Close()
        {
            BaseStream.Close();
        }
    }
}
