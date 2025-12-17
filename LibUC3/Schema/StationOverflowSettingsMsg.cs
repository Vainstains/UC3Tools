using VProto;

namespace LibUC3.Schema;

public class StationOverflowSettingsMsg : Message
{
    [ProtoField(1)] public float StationTrainStartApplyPercent;
    [ProtoField(2)] public float StationTrainEndApplyPercent;
    [ProtoField(3)] public int TrainCircuitCount;
    [ProtoField(4)] public float MinHoldDuration;
    [ProtoField(5)] public bool CanHaveTrainAtStart;
}