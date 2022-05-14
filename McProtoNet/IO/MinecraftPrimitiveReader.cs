﻿using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace McProtoNet.IO
{
    public class MinecraftPrimitiveReader : IMinecraftPrimitiveReader
    {
        private readonly Stream _data;

        public MinecraftPrimitiveReader(Stream data)
        {
            _data = data;
        }

        private int offset;

        private Span<byte> Read(int length)
        {
            byte[] buffer = new byte[length];
            //_data.Position -= length;
            _data.Read(buffer, 0, length);
            return buffer;
        }

        public sbyte ReadSignedByte() => (sbyte)this.ReadUnsignedByte();

        public ulong[] ReadULongArray()
        {
            int len = this.ReadVarInt();
            Span<byte> buffer = this.Read(len * 8);

            Span<ulong> result = MemoryMarshal.Cast<byte, ulong>(buffer);
            return result.ToArray();
        }
        public byte ReadUnsignedByte()
        {
            Span<byte> buffer = this.Read(1);

            return buffer[0];
        }

        public Guid ReadGuid()
        {
            return GuidFromTwoLong(ReadLong(), ReadLong());
        }
        private static unsafe Guid GuidFromTwoLong(long x, long y)
        {
            long* ptr = stackalloc long[2];
            ptr[0] = x;
            ptr[1] = y;
            return *(Guid*)ptr;
        }

        public bool ReadBoolean()
        {
            return ReadUnsignedByte() == 0x01;
        }




        public ushort ReadUnsignedShort()
        {
            Span<byte> buffer = this.Read(2);

            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }




        public short ReadShort()
        {
            Span<byte> buffer = this.Read(2);
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }




        public int ReadInt()
        {
            Span<byte> buffer = this.Read(4);


            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }




        public long ReadLong()
        {
            Span<byte> buffer = this.Read(8);
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }




        public ulong ReadUnsignedLong()
        {
            Span<byte> buffer = this.Read(8);
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }




        public float ReadFloat()
        {
            Span<byte> buffer = this.Read(4);
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }



        public double ReadDouble()
        {
            Span<byte> buffer = this.Read(8);
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }



        public string ReadString(int maxLength = 32767)
        {
            var length = ReadVarInt();
            var buffer = this.Read(length);

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
            return Read(len).ToArray();
        }

        public int ReadVarInt()
        {
            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                read = this.ReadUnsignedByte();

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
            int numRead = 0;
            long result = 0;
            byte read;
            do
            {
                read = this.ReadUnsignedByte();
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
            throw new NotImplementedException();
        }

    }
}
