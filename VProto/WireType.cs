namespace VProto.Raw;

internal enum WireType
{
    Varint = 0,
    Fixed64 = 1,
    LengthDelimited = 2,
    StartGroup = 3, // boooooo
    EndGroup = 4, // get outttt
    Fixed32 = 5
}