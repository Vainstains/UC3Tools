using VProto;

namespace LibUC3.Schema;

public class TrackVisualSettingsMsg : Message
{
    public enum LeftOrRightStatus
    {
        NeitherLeftOrRight = 0,
        Left = 1,
        Right = 2,
        LeftAndRight = 3
    }

    public enum StartOrEndStatus
    {
        NeitherStartOrEnd = 0,
        Start = 1,
        End = 2,
        StartAndEnd = 3
    }

    public enum TrussType
    {
        None = 0,
        Triangle = 1,
        Quad = 2,
        SingleSpine = 3,
        DoubleSpine = 4,
        SingleToDoubleSpine = 5,
        DoubleToSingleSpine = 6
    }

    [ProtoField(1)] public float DesiredBraceSpacingStart;
    [ProtoField(2)] public float DesiredBraceSpacingEnd;
    [ProtoField(9)] public LeftOrRightStatus GenerateCatwalk;
    [ProtoField(10)] public LeftOrRightStatus GenerateNet;
    [ProtoField(13)] public float SpineThicknessStart;
    [ProtoField(14)] public float SpineThicknessEnd;
    [ProtoField(15)] public StartOrEndStatus ExtendSpineToGround;
    [ProtoField(19)] public float SpineAltColorRatioStart;
    [ProtoField(20)] public float SpineAltColorRatioEnd;
    [ProtoField(21)] public TrussType TrussTypeValue;
    [ProtoField(22)] public bool DoubleTubularSpines;
    [ProtoField(23)] public string CrosstieStyle;
}