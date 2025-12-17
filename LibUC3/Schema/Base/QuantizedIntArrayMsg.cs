using VProto;

namespace LibUC3.Schema;

#pragma warning disable CS8618
public class QuantizedIntArrayMsg : Message
{
    [ProtoField(1)] public byte[] Data;
    [ProtoField(2)] public int NumInts;
    [ProtoField(3)] public int NumBitsPerInt;
    [ProtoField(4)] public int MinValue;
}