using VProto;

namespace LibUC3.Schema;

public class ChainLiftOverflowSettingsMsg : Message
{
    [ProtoField(1)] public float ChainLiftTrainStartApplyPercent;
    [ProtoField(2)] public float ChainLiftTrainEndApplyPercent;
    [ProtoField(3)] public bool IsBlockSection;
    [ProtoField(4)] public float DropChainHoldDuration;
    [ProtoField(5)] public bool CanHaveTrainAtStart;
}