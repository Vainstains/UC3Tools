using System;
using System.Collections.Generic;
using System.IO;

namespace VProto.Raw;

internal static class Utils
{
    public static List<byte> EncodeVarint(ulong value)
    {
        var bytes = new List<byte>();
        do
        {
            var b = (byte)(value & 0x7F);
            value >>= 7;
            if (value != 0)
                b |= 0x80;
            bytes.Add(b);
        } while (value != 0);
        return bytes;
    }
    
    public static ulong DecodeVarint(ReadState state)
    {
        ulong result = 0;
        var shift = 0;

        while (state.HasNext)
        {
            var b = state.Next();
            result |= ((ulong)(b & 0x7F)) << shift;

            if ((b & 0x80) == 0)
                return result;

            shift += 7;
            if (shift >= 64)
                throw new FormatException("Varint is too long for a 64-bit integer");
        }

        throw new EndOfStreamException("Unexpected end of data while parsing varint");
    }
    
    public static ulong DecodeVarint(ReadState state, out byte[] raw)
    {
        ulong result = 0;
        var shift = 0;
        var rawBytes = new List<byte>();

        while (state.HasNext)
        {
            var b = state.Next();
            rawBytes.Add(b);
            result |= ((ulong)(b & 0x7F)) << shift;

            if ((b & 0x80) == 0)
            {
                raw = rawBytes.ToArray();
                return result;
            }

            shift += 7;
            if (shift >= 64)
                throw new FormatException("Varint is too long for a 64-bit integer");
        }

        throw new EndOfStreamException("Unexpected end of data while parsing varint");
    }
    
    public static ulong ZigZagEncode(long value)
    {
        return (ulong)((value << 1) ^ (value >> 63));
    }
    
    public static long ZigZagDecode(ulong value)
    {
        return (long)((value >> 1) ^ (~(value & 1) + 1));
    }
}