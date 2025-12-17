using VProto;

namespace LibUC3.Schema;

public class StructureGenerationSettingsMsg : Message
{
    [ProtoField(1)] public SegmentStructure.Type Type;

    public class StationPlatformSettingsMsg : Message
    {
        [ProtoField(1)] public float LeftPlatformWidth;
        [ProtoField(2)] public float RightPlatformWidth;
        [ProtoField(3)] public float PlatformHeight;
    }

    [ProtoField(2)] public StationPlatformSettingsMsg StationPlatformSettings;

    public class TunnelSettingsMsg : Message
    {
        
    }

    [ProtoField(3)] public TunnelSettingsMsg TunnelSettings;
}