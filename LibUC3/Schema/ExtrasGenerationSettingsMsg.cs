using VProto;

namespace LibUC3.Schema;

public class ExtrasGenerationSettingsMsg : Message
{
    public class ExtraGenerationSettingMsg : Message
    {
        [ProtoField(1)] public SegmentExtra.Type Type;
        [ProtoField(2)] public float HorizontalOffset;
        [ProtoField(3)] public int PatternNumerator;
        [ProtoField(4)] public int PatternDenominator;
        [ProtoField(5)] public int PatternOffset;
    }

    [ProtoField(1)] public List<ExtraGenerationSettingMsg> ExtraGenerationSettings = new();
}