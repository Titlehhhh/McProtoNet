namespace McProtoNet.Core.Protocol
{
    public static class Extensions
    {

        public static int GetVarIntLength(this int val)
        {
            int amount = 0;
            do
            {
                val >>= 7;
                amount++;
            } while (val != 0);

            return amount;
        }
        public static int GetVarIntLength(this int value, byte[] data)
        {
            var unsigned = (uint)value;

            int len = 0;
            do
            {
                var temp = (byte)(unsigned & 127);
                unsigned >>= 7;

                if (unsigned != 0)
                    temp |= 128;

                data[len++] = temp;
            }
            while (unsigned != 0);
            return len;
        }

        public static int GetVarIntLength(this int value, Span<byte> data)
        {
            var unsigned = (uint)value;

            int len = 0;
            do
            {
                var temp = (byte)(unsigned & 127);
                unsigned >>= 7;

                if (unsigned != 0)
                    temp |= 128;

                data[len++] = temp;
            }
            while (unsigned != 0);
            return len;
        }

        public static int ReadVarInt(this Stream stream)
        {
            byte[] buff = new byte[1];

            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                while (stream.Read(buff, 0, 1) == 0) ;
                read = buff[0];


                int value = read & 0b01111111;
                result |= value << 7 * numRead;

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidOperationException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);

            return result;
        }

        public static async ValueTask<int> ReadVarIntAsync(this Stream stream, CancellationToken token = default)
        {
            byte[] buff = new byte[1];

            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                await stream.ReadAsync(buff, token);
                read = buff[0];


                int value = read & 0b01111111;
                result |= value << 7 * numRead;

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidOperationException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);

            return result;
        }
        public static int ReadVarInt(this Stream stream, out int len)
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
                result |= value << 7 * numRead;

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidOperationException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);
            len = (byte)numRead;
            return result;
        }
        public static void WriteVarInt(this Stream stream, int value)
        {
            var unsigned = (uint)value;
            //  byte[] data = new byte[5];
            //  int len = 0;
            do
            {
                var temp = (byte)(unsigned & 127);
                unsigned >>= 7;

                if (unsigned != 0)
                    temp |= 128;

                stream.WriteByte(temp);
                //data[len++] = temp;
            }
            while (unsigned != 0);

            //  stream.Write(data, 0, len);
        }
        public static Task WriteVarIntAsync(this Stream stream, int value, CancellationToken token = default)
        {
            var unsigned = (uint)value;
            byte[] data = new byte[5];
            int len = 0;
            do
            {
                token.ThrowIfCancellationRequested();
                var temp = (byte)(unsigned & 127);
                unsigned >>= 7;

                if (unsigned != 0)
                    temp |= 128;
                data[len++] = temp;
            }
            while (unsigned != 0);
            return stream.WriteAsync(data, 0, len, token);
        }


        public static int ReadToEnd(this Stream stream, Span<byte> buffer, int length)
        {
            int totalRead = 0;
            while (totalRead < length)
            {
                int read = stream.Read(buffer.Slice(totalRead));


                totalRead += read;
            }

            return totalRead;
        }
        public static async ValueTask<int> ReadToEndAsync(this Stream stream, Memory<byte> buffer, int length, CancellationToken token)
        {
            int totalRead = 0;
            while (totalRead < length)
            {
                int read = await stream.ReadAsync(buffer.Slice(totalRead), token);


                totalRead += read;
            }

            return totalRead;
        }
    }
}
