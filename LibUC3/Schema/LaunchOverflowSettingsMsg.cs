using VProto;

namespace LibUC3.Schema;

public class LaunchOverflowSettingsMsg : Message
{
    [ProtoField(1)] public float LaunchTrainStartApplyPercent;
    [ProtoField(2)] public float LaunchTrainEndApplyPercent;
    [ProtoField(3)] public float HoldingBrakeDuration;
    [ProtoField(4)] public bool CanHaveTrainAtStart;
}