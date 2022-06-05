using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Utils.Dns
{
    public class RecordReader
    {
        private byte[] m_Data;
        private int m_Position;
        public RecordReader(byte[] data)
        {
            m_Data = data;
            m_Position = 0;
        }

        public int Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }

        public RecordReader(byte[] data, int Position)
        {
            m_Data = data;
            m_Position = Position;
        }


        public byte ReadByte()
        {
            if (m_Position >= m_Data.Length)
                return 0;
            return m_Data[m_Position++];
        }

        public char ReadChar()
        {
            return (char)ReadByte();
        }

        public UInt16 ReadUInt16()
        {
            return (UInt16)(ReadByte() << 8 | ReadByte());
        }

        public UInt16 ReadUInt16(int offset)
        {
            m_Position += offset;
            return ReadUInt16();
        }

        public UInt32 ReadUInt32()
        {
            return (UInt32)(ReadUInt16() << 16 | ReadUInt16());
        }

        public string ReadDomainName()
        {
            StringBuilder name = new StringBuilder();
            int length = 0;

            // get  the length of the first label
            while ((length = ReadByte()) != 0)
            {
                // top 2 bits set denotes domain name compression and to reference elsewhere
                if ((length & 0xc0) == 0xc0)
                {
                    // work out the existing domain name, copy this pointer
                    RecordReader newRecordReader = new RecordReader(m_Data, (length & 0x3f) << 8 | ReadByte());

                    name.Append(newRecordReader.ReadDomainName());
                    return name.ToString();
                }

                // if not using compression, copy a char at a time to the domain name
                while (length > 0)
                {
                    name.Append(ReadChar());
                    length--;
                }
                name.Append('.');
            }
            if (name.Length == 0)
                return ".";
            else
                return name.ToString();
        }

        public string ReadString()
        {
            short length = this.ReadByte();

            StringBuilder name = new StringBuilder();
            for (int intI = 0; intI < length; intI++)
                name.Append(ReadChar());
            return name.ToString();
        }

        public byte[] ReadBytes(int intLength)
        {
            var result = new byte[intLength];
            Array.Copy(m_Data, m_Position, result, 0, intLength);
            m_Position += intLength;
            return result;
        }

        public RecordSRV ReadSrv(int Length)
        {
            return new RecordSRV(this);
        }

    }
}
