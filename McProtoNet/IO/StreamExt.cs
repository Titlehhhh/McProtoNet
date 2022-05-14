﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.IO
{
    public static class StreamExt
    {
        

        public static int ReadVarInt(this Stream stream)
        {
            byte[] buff = new byte[1];

            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                stream.Read(buff, 0, 1);
                read = buff[0];


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

        public static void WriteVarInt(this Stream stream,int value)
        {
            var unsigned = (uint)value;
            do
            {
                var temp = (byte)(unsigned & 127);
                unsigned >>= 7;

                if (unsigned != 0)
                    temp |= 128;

                stream.WriteByte(temp);
            }
            while (unsigned != 0);
        }
    }
}
