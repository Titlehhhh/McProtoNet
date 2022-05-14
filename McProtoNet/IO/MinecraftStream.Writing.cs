using McProtoNet.NBT;
using McProtoNet.Utils;
using System.Buffers.Binary;
using System.Text;

namespace McProtoNet.IO
{

    public sealed partial class MinecraftStream
    {
        public   void WriteUnsignedLong(ulong value)
        {
            Span<byte> span = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(span, value);
            BaseStream.Write(span);
        }

        public   void WriteULongArray(ulong[] value)
        {
            throw new NotImplementedException();
        }

        public   void WriteByte(sbyte value)
        {
            BaseStream.WriteByte((byte)value);
        }




        public   void WriteUnsignedByte(byte value)
        {
            BaseStream.WriteByte(value);
        }




        public   void WriteBoolean(bool value)
        {
            BaseStream.WriteByte((byte)(value ? 0x01 : 0x00));
        }




        public   void WriteUnsignedShort(ushort value)
        {
            Span<byte> span = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(span, value);
            BaseStream.Write(span);
        }




        public   void WriteShort(short value)
        {
            Span<byte> span = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(span, value);
            BaseStream.Write(span);
        }




        public   void WriteInt(int value)
        {
            Span<byte> span = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(span, value);
            BaseStream.Write(span);
        }




        public   void WriteLong(long value)
        {
            Span<byte> span = stackalloc byte[8];
            BinaryPrimitives.WriteInt64BigEndian(span, value);
            BaseStream.Write(span);
        }




        public   void WriteFloat(float value)
        {
            Span<byte> span = stackalloc byte[4];
            BinaryPrimitives.WriteSingleBigEndian(span, value);
            BaseStream.Write(span);
        }




        public   void WriteDouble(double value)
        {
            Span<byte> span = stackalloc byte[8];
            BinaryPrimitives.WriteDoubleBigEndian(span, value);
            BaseStream.Write(span);
        }




        public   void WriteString(string value, int maxLength = short.MaxValue)
        {

            using var bytes = new RentedArray<byte>(Encoding.UTF8.GetByteCount(value));
            Encoding.UTF8.GetBytes(value, bytes.Span);
            WriteVarInt(bytes.Length);
            Write(bytes);
        }




        public   void WriteVarInt(int value)
        {
            var unsigned = (uint)value;
            do
            {
                var temp = (byte)(unsigned & 127);
                unsigned >>= 7;

                if (unsigned != 0)
                    temp |= 128;

                BaseStream.WriteByte(temp);
            }
            while (unsigned != 0);
        }

        public   void WriteVarInt(Enum value)
        {
            WriteVarInt(Convert.ToInt32(value));
        }

        public   void WriteLongArray(long[] values)
        {
            Span<byte> buffer = stackalloc byte[8];
            for (int i = 0; i < values.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer, values[i]);
                BaseStream.Write(buffer);
            }
        }






        public   void WriteVarLong(long value)
        {
            var unsigned = (ulong)value;

            do
            {
                var temp = (byte)(unsigned & 127);

                unsigned >>= 7;

                if (unsigned != 0)
                    temp |= 128;


                BaseStream.WriteByte(temp);
            }
            while (unsigned != 0);
        }






        public   void WriteByteArray(byte[] values)
        {
            BaseStream.Write(values);
        }


        public   void WriteUuid(Guid value)
        {
            if (value == Guid.Empty)
            {
                WriteLong(0L);
                WriteLong(0L);
            }
            else
            {
                var uuid = System.Numerics.BigInteger.Parse(value.ToString().Replace("-", ""), System.Globalization.NumberStyles.HexNumber);
                Write(uuid.ToByteArray(false, true));
            }
        }

        public   void WriteNbt(NbtTag nbt)
        {
            var writer = new NbtWriter(BaseStream, nbt.Name);
            writer.WriteTag(nbt);
        }
        public   void WriteNbtCompound(NbtCompound compound)
        {
            var writer = new NbtWriter(BaseStream, compound.Name);
            writer.WriteTag(compound);
        }

        public   void Write(byte[] buffer)
        {
            this.Write(buffer);
        }
    }
}