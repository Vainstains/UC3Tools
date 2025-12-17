using VProto;

namespace LibUC3.Schema;

public class SupportVisualSettingsMsg : Message
{
    public enum FooterTextureEnum
    {
        NoFooterTexture,
        Concrete
    }
    
    public enum SupportTextureEnum
    {
        NoSupportTexture,
        WoodGrain,
        Metal
    }
    
    public enum SupportTypeEnum
    {
        NoSupport,
        Tube,
        Lattice
    }

    public enum SupportFooterTypeEnum
    {
        NoFooter,
        Circle,
        Square
    }
    
    public enum SupportTubeTypeEnum
    {
        Cylinder,
        Rectangle
    }

    public class TubeSupportSettingsMsg : Message
    {
        public enum TubeSupportTypeEnum
        {
            VerticalBeam,
            DualBeam
        }
        
        [ProtoField(2)] public SupportTubeTypeEnum SupportTubeType;
        [ProtoField(4)] public float HalfSize;
        [ProtoField(5)] public TubeSupportTypeEnum TubeSupportType;
        [ProtoField(6)] public float RollAngle;
        [ProtoField(7)] public float OutAngle;
        [ProtoField(8)] public bool FlipRoll;
        [ProtoField(9)] public float SecondBeamOutOffset;
        [ProtoField(10)] public float SupportTrackConnectNormalOffset;
        [ProtoField(11)] public float SupportOverhangConnectOffset;
        [ProtoField(12)] public float SupportTrackConnectOffsetOuter;
        [ProtoField(13)] public SupportTextureEnum SupportTexture;
        [ProtoField(14)] public FooterTextureEnum FooterTexture;
        [ProtoField(15)] public float FooterHeight;
        [ProtoField(16)] public SupportFooterTypeEnum SupportFooterType;
        [ProtoField(17)] public float FooterHalfSize;
        [ProtoField(18)] public float TubeJointSpacing;
    }

    public class LatticeSupportSettingsMsg : Message
    {
        public class BackSettingsMsg : Message
        {
            public enum OffsetModeEnum
            {
                StartOfSegment,
                ArcLength,
                XZDistance,
                PreviousSupport
            }

            [ProtoField(1)] public LatticeSupportSettingsMsg BackSupportSettings;
            [ProtoField(2)] public OffsetModeEnum OffsetMode;
            [ProtoField(3)] public float ArcLengthDistance;
            [ProtoField(4)] public float XZDistance;
        }
        
        public enum SlopeTypeEnum
        {
            AtTop,
            AtGrid
        }

        public enum ConnectionModeEnum
        {
            None,
            ToPrevious
        }
        
        [ProtoField(1)] public float HalfSize;
        [ProtoField(2)] public float SlopeAngle;
        [ProtoField(3)] public float VerticalSpacing;
        [ProtoField(4)] public float SlopeSpacingVerticalGridSize;
        [ProtoField(5)] public BackSettingsMsg BackSettings;
        [ProtoField(6)] public SupportTextureEnum SupportTexture;
        [ProtoField(7)] public FooterTextureEnum FooterTexture;
        [ProtoField(8)] public SupportTubeTypeEnum SupportTubeType;
        [ProtoField(9)] public bool IsIBeamFramed;
        [ProtoField(10)] public int ExtendRightColumns;
        [ProtoField(11)] public int ExtendLeftColumns;
        [ProtoField(12)] public SlopeTypeEnum SlopeType;
        [ProtoField(13)] public float ColumnOutwardSpacing;
        [ProtoField(14)] public int MaxRightColumns;
        [ProtoField(15)] public int MaxLeftColumns;
        [ProtoField(16)] public float HandrailHeight;
        [ProtoField(17)] public float FooterHeight;
        [ProtoField(18)] public SupportFooterTypeEnum SupportFooterType;
        [ProtoField(19)] public float FooterHalfSize;
        [ProtoField(20)] public ConnectionModeEnum ConnectionMode;
    }
    
    [ProtoField(2)] public SupportTypeEnum SupportType;
    [ProtoField(4)] public TubeSupportSettingsMsg TubeSupportSettings;
    [ProtoField(8)] public LatticeSupportSettingsMsg LatticeSupportSettings;
}