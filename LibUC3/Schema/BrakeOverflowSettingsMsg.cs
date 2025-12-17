using VProto;

namespace LibUC3.Schema;

public class BrakeOverflowSettingsMsg : Message
{
    [ProtoField(1)] public float BrakeTrainStartApplyPercent;
    [ProtoField(2)] public float BrakeTrainEndApplyPercent;
    [ProtoField(3)] public float HoldingBrakeDuration;
    [ProtoField(4)] public bool CanHaveTrainAtStart;
}