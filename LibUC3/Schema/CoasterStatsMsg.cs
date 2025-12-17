using VProto;

namespace LibUC3.Schema;

public class CoasterStatsMsg : Message
{
    [ProtoField(1)] public float Length;
    [ProtoField(2)] public float Height;
    [ProtoField(3)] public float LargestDropHeight;
    [ProtoField(4)] public List<float> DropHeights;
    [ProtoField(5)] public int NumInversions;
    [ProtoField(6)] public float MaxSpeed;
    [ProtoField(7)] public float MinSpeed;
    [ProtoField(8)] public float MaxPositiveG;
    [ProtoField(9)] public float MaxNegativeG;
    [ProtoField(10)] public float MaxLateralG;
    [ProtoField(11)] public float AverageSpeed;
    [ProtoField(12)] public float Duration;
    [ProtoField(13)] public float MaxAcceleration;
    [ProtoField(14)] public float MaxDeceleration;
}