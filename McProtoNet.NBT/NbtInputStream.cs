using System.IO.Compression;
using System.Text;

namespace McProtoNet.NBT
{
    public class NbtInputStream
    {
        public Stream BaseStream { get; set; }

        public NbtInputStream(Stream baseStream)
        {
            BaseStream = new GZipStream(baseStream, CompressionLevel.NoCompression);
        }

        public NbtInputStream(Stream baseStream, bool gzip)
        {
            if (gzip)
            {
                BaseStream = new GZipStream(baseStream, CompressionLevel.Optimal);
            }

            BaseStream = baseStream;
        }

        public NbtTag Read()
        {
            return Read(0);
        }

        private NbtTag Read(int depth)
        {
            NbtTagType type = (NbtTagType)(BaseStream.ReadByte() & 0xFF);
            string name;
            BinaryReader br = new(BaseStream);
            if (type != NbtTagType.End)
            {
                int nameLength = br.ReadInt16() & 0xFFFF;
                byte[] nameBytes = new byte[nameLength];
                name = Encoding.UTF8.GetString(nameBytes);
            }
            else
            {
                name = "";
            }

            return ReadTagPayload(br, type, name, depth);
        }

        private NbtTag ReadTagPayload(BinaryReader br, NbtTagType type, string name, int depth, NbtTag tag = null)
        {
            switch (type)
            {
                case NbtTagType.End:
                    if (depth == 0)
                    {
                        throw new Exception("Invalid operation.");
                    }
                    else
                    {
                        return new NbtEnd();
                    }
                case NbtTagType.Byte:
                    return new NbtByte(name, br.ReadByte());
                case NbtTagType.Short:
                    return new NbtShort(name, br.ReadInt16());
                case NbtTagType.Int:
                    return new NbtInt(name, br.ReadInt32());
                case NbtTagType.Long:
                    return new NbtLong(name, br.ReadInt64());
                case NbtTagType.Float:
                    return new NbtFloat(name, br.ReadSingle());
                case NbtTagType.Double:
                    return new NbtDouble(name, br.ReadDouble());
                case NbtTagType.ByteArray:
                    int length = br.ReadInt32();
                    return new NbtByteArray(name, br.ReadBytes(length));
                case NbtTagType.String:
                    length = br.ReadInt16();
                    return new NbtString(name, Encoding.UTF8.GetString(br.ReadBytes(length)));
                case NbtTagType.List:
                    int childType = br.ReadByte();
                    length = br.ReadInt32();

                    List<NbtTag> tagist = new();
                    for (int i = 0; i < length; i++)
                    {
                        NbtTag t = ReadTagPayload(br, (NbtTagType)childType, "", depth + 1);
                        tagist.Add(t);
                    }

                    return new NbtList(tagist, (NbtTagType)childType);
                case NbtTagType.Compound:
                    Dictionary<string, NbtTag> tagMap = new();
                    while (true)
                    {
                        NbtTag tt = Read(depth + 1);
                        if (tt.TagType == NbtTagType.End)
                        {
                            break;
                        }
                        else
                        {
                            tagMap.Add(tt.Name, tt);
                        }
                    }

                    NbtCompound cmp = new(tag.Name);
                    foreach (NbtTag f in tagMap.Values)
                    {
                        cmp.Add(f);
                    }

                    return cmp;
                case NbtTagType.IntArray:
                    length = br.ReadInt32();
                    int[] ints = new int[length];
                    for (int i = 0; i < length; i++)
                    {
                        ints[i] = br.ReadInt32();
                    }

                    return new NbtIntArray(name, ints);
                case NbtTagType.LongArray:
                    length = br.ReadInt32();
                    long[] longs = new long[length];
                    for (int i = 0; i < length; i++)
                    {
                        longs[i] = br.ReadInt64();
                    }

                    return new NbtLongArray(name, longs);
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
