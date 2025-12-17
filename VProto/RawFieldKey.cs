namespace VProto.Raw;

internal struct RawFieldKey
{
    public readonly WireType Type;
    public readonly int FieldNumber;

    public RawFieldKey(WireType type, int fieldNumber)
    {
        Type = type;
        FieldNumber = fieldNumber;
    }

    public RawFieldKey Parse(ReadState state)
    {
        var key = (int)Utils.DecodeVarint(state);
        var number = key >> 3;
        var type = (WireType)(key & 0x07);
        return new RawFieldKey(type, number);
    }

    public readonly void Encode(WriteState state)
    {
        var key = (FieldNumber << 3) | (int)Type;
        state.Write(Utils.EncodeVarint((ulong)key));
    }
}