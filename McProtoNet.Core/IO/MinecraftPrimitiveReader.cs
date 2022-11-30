using McProtoNet.Core.Helpers;
using McProtoNet.NBT;
using System.Buffers.Binary;
using System.Text;

namespace McProtoNet.Core.IO
{
    public class MinecraftPrimitiveReader : IMinecraftPrimitiveReader
    {
        public Stream BaseStream { get; set; }

        public MinecraftPrimitiveReader(Stream stream)
        {
            BaseStream = stream;
        }
        public MinecraftPrimitiveReader()
        {

        }



        public virtual sbyte ReadSignedByte() => (sbyte)this.ReadUnsignedByte();

        public virtual ulong[] ReadULongArray()
        {
            int len = this.ReadVarInt();

            ulong[] result = new ulong[len];

            for (int i = 0; i < len; i++)
                result[i] = ReadULong();
            return result;

        }

        public virtual long[] ReadLongArray()
        {
            int len = this.ReadVarInt();

            long[] result = new long[len];
            for (int i = 0; i < len; i++)
                result[i] = ReadLong();
            return result;
        }
        public virtual byte ReadUnsignedByte()
        {
            Span<byte> buffer = stackalloc byte[1];
            BaseStream.Read(buffer);

            return buffer[0];
        }

        public virtual Guid ReadUUID()
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

        public virtual bool ReadBoolean()
        {
            return ReadUnsignedByte() == 0x01;
        }




        public virtual ushort ReadUnsignedShort()
        {
            Span<byte> buffer = stackalloc byte[2];
            BaseStream.Read(buffer);

            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }




        public virtual short ReadShort()
        {
            Span<byte> buffer = stackalloc byte[2];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }




        public virtual int ReadInt()
        {
            Span<byte> buffer = stackalloc byte[4];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }




        public virtual long ReadLong()
        {
            Span<byte> buffer = stackalloc byte[8];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }
        public virtual ulong ReadULong()
        {
            Span<byte> buffer = stackalloc byte[8];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }



        public virtual ulong ReadUnsignedLong()
        {
            Span<byte> buffer = stackalloc byte[8];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }




        public virtual float ReadFloat()
        {
            Span<byte> buffer = stackalloc byte[4];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }



        public virtual double ReadDouble()
        {
            Span<byte> buffer = stackalloc byte[8];
            BaseStream.Read(buffer);
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }



        public virtual string ReadString(int maxLength = 32767)
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



        public virtual byte[] ReadByteArray()
        {
            int len = ReadVarInt();
            Span<byte> buffer = stackalloc byte[len];
            BaseStream.Read(buffer);
            return buffer.ToArray();
        }
        public virtual byte[] ReadByteArray(int size)
        {
            Span<byte> buffer = stackalloc byte[size];
            BaseStream.Read(buffer);
            return buffer.ToArray();
        }

        public virtual int ReadVarInt()
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
        public virtual long ReadVarLong()
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

        public virtual byte[] ReadToEnd()
        {
            using (var ms = new MemoryStream())
            {
                BaseStream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public virtual NbtCompound? ReadNbt()
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
