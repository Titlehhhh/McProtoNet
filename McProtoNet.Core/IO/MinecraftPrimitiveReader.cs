using McProtoNet.Core.Helpers;
using McProtoNet.NBT;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace McProtoNet.Core.IO
{
    public sealed class MinecraftPrimitiveReader : IMinecraftPrimitiveReader
    {
        public Stream BaseStream { get; set; }

        public MinecraftPrimitiveReader(Stream stream)
        {
            BaseStream = stream;
        }
        public MinecraftPrimitiveReader()
        {

        }



        public sbyte ReadSignedByte() => (sbyte)this.ReadUnsignedByte();

        public ulong[] ReadULongArray()
        {
            int len = this.ReadVarInt();

            ulong[] result = new ulong[len];

            for (int i = 0; i < len; i++)
                result[i] = ReadULong();
            return result;

        }

        public long[] ReadLongArray()
        {
            int len = this.ReadVarInt();

            long[] result = new long[len];
            for (int i = 0; i < len; i++)
                result[i] = ReadLong();
            return result;
        }
        public byte ReadUnsignedByte()
        {
            Span<byte> buffer = stackalloc byte[1];
            BaseStream.Read(buffer);

            return buffer[0];
        }

        public Guid ReadUUID()
        {
            Guid guid = GuidFromTwoLong();
            if (BitConverter.IsLittleEndian)
                guid = guid.ToLittleEndian();
            //  else
            //       guid = guid.ToBigEndian();
            return guid;
        }
        private Guid GuidFromTwoLong()
        {
            Span<byte> buffer = stackalloc byte[2 * 8];
            BaseStream.Read(buffer);
            return new Guid(buffer);
        }

        public bool ReadBoolean()
        {
            return ReadUnsignedByte() == 0x01;
        }




        public ushort ReadUnsignedShort()
        {
            Span<byte> buffer = stackalloc byte[2];
            BaseStream.Read(buffer);

            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }




        public short ReadShort()
        {
            Span<byte> buffer = stackalloc byte[2];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }




        public int ReadInt()
        {
            Span<byte> buffer = stackalloc byte[4];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }




        public long ReadLong()
        {
            Span<byte> buffer = stackalloc byte[8];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }
        public ulong ReadULong()
        {
            Span<byte> buffer = stackalloc byte[8];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }



        public ulong ReadUnsignedLong()
        {
            Span<byte> buffer = stackalloc byte[8];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }




        public float ReadFloat()
        {
            Span<byte> buffer = stackalloc byte[4];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }



        public double ReadDouble()
        {
            Span<byte> buffer = stackalloc byte[8];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }



        public string ReadString(int maxLength = 32767)
        {
            var length = ReadVarInt();
            byte[] buffer = new byte[length];
            BaseStream.Read(buffer, 0, length);

            var value = Encoding.UTF8.GetString(buffer);
            if (maxLength > 0 && value.Length > maxLength)
            {
                throw new ArgumentException($"string ({value.Length}) exceeded maximum length ({maxLength})", nameof(value));
            }
            return value;
        }



        public byte[] ReadByteArray()
        {
            int len = ReadVarInt();
            Span<byte> buffer = stackalloc byte[len];
            BaseStream.Read(buffer);
            return buffer.ToArray();
        }
        public byte[] ReadByteArray(int size)
        {
            Span<byte> buffer = stackalloc byte[size];
            BaseStream.Read(buffer);
            return buffer.ToArray();
        }

        public int ReadVarInt()
        {
            Span<byte> buffer = stackalloc byte[1];

            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                BaseStream.Read(buffer);
                read = buffer[0];

                int value = read & 0b01111111;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidOperationException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);

            return result;
        }
        public long ReadVarLong()
        {
            Span<byte> buffer = stackalloc byte[1];

            int numRead = 0;
            long result = 0;
            byte read;
            do
            {
                BaseStream.Read(buffer);
                read = buffer[0];

                int value = (read & 0b01111111);
                result |= (long)value << (7 * numRead);

                numRead++;
                if (numRead > 10)
                {
                    throw new InvalidOperationException("VarLong is too big");
                }
            } while ((read & 0b10000000) != 0);

            return result;
        }

        public byte[] ReadToEnd()
        {
            using (var ms = new MemoryStream())
            {
                BaseStream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public NbtCompound? ReadNbt()
        {
            var nbtreader = new NbtReader(BaseStream);
            NbtCompound? result = null;
            try
            {
                result = (NbtCompound)nbtreader.ReadAsTag();
            }
            catch (NbtFormatException)
            {
                return null;
            }

            return result;
        }
    }
}
