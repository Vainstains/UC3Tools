using System;
using VProto.Raw;

namespace VProto;

internal class RawField
{
    public readonly RawFieldKey Key;
    public readonly byte[] Data;

    public RawField(RawFieldKey key, byte[] data)
    {
        Key = key;
        Data = data;
    }

    public static RawField Parse(ReadState state)
    {
        var key = new RawFieldKey().Parse(state);

        byte[] data;

        switch (key.Type)
        {
            case WireType.Varint:
                Utils.DecodeVarint(state, out data);
                break;

            case WireType.Fixed64:
                data = state.ReadBytes(8);
                break;

            case WireType.LengthDelimited:
                var length = (int)Utils.DecodeVarint(state);
                data = state.ReadBytes(length);
                break;

            case WireType.Fixed32:
                data = state.ReadBytes(4);
                break;

            default:
                throw new InvalidOperationException($"Unknown wire type: {key.Type}");
        }

        return new RawField(key, data);
    }

    public void Encode(WriteState state)
    {
        Key.Encode(state);

        switch (Key.Type)
        {
            case WireType.Varint:
                break;

            case WireType.Fixed64:
                if (Data.Length != 8)
                    throw new InvalidOperationException("Fixed64 field must be exactly 8 bytes.");
                break;

            case WireType.LengthDelimited:
                state.Write(Utils.EncodeVarint((ulong)Data.Length));
                break;

            case WireType.Fixed32:
                if (Data.Length != 4)
                    throw new InvalidOperationException("Fixed32 field must be exactly 4 bytes.");
                break;

            default:
                throw new InvalidOperationException($"Unknown wire type: {Key.Type}");
        }
        
        state.Write(Data);
    }
}