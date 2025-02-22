using System.Runtime.Intrinsics;
using System.Text;

namespace McProtoNet.Serialization;

public class MUTF8Encoding : Encoding
{
    public override byte[] GetBytes(string s)
    {
        if (string.IsNullOrEmpty(s))
            return [];

        int byteCount = 0;
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (c == 0)
                byteCount += 2;
            else if (c >= 0x0001 && c <= 0x007F)
                byteCount += 1;
            else if (c <= 0x07FF)
                byteCount += 2;
            else
            {
                if (char.IsHighSurrogate(c) && i + 1 < s.Length && char.IsLowSurrogate(s[i + 1]))
                {
                    byteCount += 6;
                    i++;
                }
                else
                {
                    byteCount += 3;
                }
            }
        }


        byte[] bytes = new byte[byteCount];
        int position = 0;

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            if (c == 0)
            {
                bytes[position++] = 0xC0;
                bytes[position++] = 0x80;
            }
            else if (c >= 0x0001 && c <= 0x007F)
            {
                bytes[position++] = (byte)c;
            }
            else if (c <= 0b11111111111)
            {
                bytes[position++] = (byte)(0b11000000 | ((c >> 6) & 0b11111));
                bytes[position++] = (byte)(0b10000000 | (c & 0b111111));
            }
            else
            {
                if (char.IsHighSurrogate(c) && i + 1 < s.Length && char.IsLowSurrogate(s[i + 1]))
                {
                    bytes[position++] = (byte)(0xE0 | ((c >> 12) & 0x0F));
                    bytes[position++] = (byte)(0x80 | ((c >> 6) & 0x3F));
                    bytes[position++] = (byte)(0x80 | (c & 0x3F));

                    char lowSurrogate = s[++i];
                    bytes[position++] = (byte)(0xE0 | ((lowSurrogate >> 12) & 0x0F));
                    bytes[position++] = (byte)(0x80 | ((lowSurrogate >> 6) & 0x3F));
                    bytes[position++] = (byte)(0x80 | (lowSurrogate & 0x3F));
                }
                else
                {
                    bytes[position++] = (byte)(0xE0 | ((c >> 12) & 0x0F));
                    bytes[position++] = (byte)(0x80 | ((c >> 6) & 0x3F));
                    bytes[position++] = (byte)(0x80 | (c & 0x3F));
                }
            }
        }

        return bytes;
    }

    public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
        return base.GetBytes(s, charIndex, charCount, bytes, byteIndex);
    }

    public override int GetByteCount(ReadOnlySpan<char> chars)
    {
        return base.GetByteCount(chars);
    }

    public override int GetByteCount(char[] chars, int index, int count)
    {
        throw new NotImplementedException();
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
        throw new NotImplementedException();
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        throw new NotImplementedException();
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
        throw new NotImplementedException();
    }

    public override int GetMaxByteCount(int charCount)
    {
        throw new NotImplementedException();
    }

    public override int GetMaxCharCount(int byteCount)
    {
        throw new NotImplementedException();
    }
}