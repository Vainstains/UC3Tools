namespace LibUC3.Schema;

public class SegmentExtra
{
    public enum Type
    {
        None = 0,
        Chain = 1,
        SingleMagneticFin = 2,
        NarrowMagneticFins = 3,
        BroadMagneticFins = 4,
        FrictionBrake = 5,
        SideFinBrake = 6,
        CableLift = 7,
        CableLaunch = 8,
        VerticalDriveTires = 9,
        HorizontalDriveTires = 10
    }

    public enum Spacing
    {
        PerBrace = 0,
        EveryOtherBrace = 1,
        OnePerThreeBraces = 2,
        TwoPerThreeBraces = 3,
        OnePerFourBraces = 4,
        TwoPerFourBraces = 5,
        ThreePerFourBraces = 6
    }

    public enum HorizontalOffset
    {
        LeftEdge = 0,
        Left = 1,
        LeftCenter = 2,
        Centered = 3,
        RightCenter = 4,
        Right = 5,
        RightEdge = 6
    }
}