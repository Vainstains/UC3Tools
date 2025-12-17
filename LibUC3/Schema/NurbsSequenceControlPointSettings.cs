using VProto;

namespace LibUC3.Schema;

public class NurbsSequenceControlPointSettings : Message
{
    [ProtoField(1)] public Vec3Msg Position;
    [ProtoField(2)] public float Weight;
    [ProtoField(3)] public bool Strict;
}