using VProto;

namespace LibUC3.Schema;

public class SegmentNodeSettingsMsg : Message
{
    public enum Type
    {
        Standard = 0,
        ChainLift = 1,
        Brake = 2,
        Launch = 3,
        Station = 4
    }

    [ProtoField(1)] public float ControlPointIndex;
    [ProtoField(2)] public Type SegmentType;
    [ProtoField(3)] public string Uuid;

    public class ChainLiftSettingsMsg : Message
    {
        [ProtoField(1)] public float Speed;
        [ProtoField(2)] public float Accel;
        [ProtoField(5)] public ChainLiftOverflowSettingsMsg OverrideChainLiftOverflowSettings;
    }

    [ProtoField(4)] public ChainLiftSettingsMsg ChainLiftSettings;

    public class StationSettingsMsg : Message
    {
        [ProtoField(1)] public float LaunchSpeed;
        [ProtoField(2)] public float LaunchAccel;
        [ProtoField(3)] public float BrakeSpeed;
        [ProtoField(4)] public float BrakeDecel;

        public enum LaunchMode
        {
            Forward = 0,
            Backward = 1
        }

        [ProtoField(6)] public LaunchMode StationLaunchMode;
        [ProtoField(8)] public StationOverflowSettingsMsg OverrideStationOverflowSettings;
    }

    [ProtoField(5)] public StationSettingsMsg StationSettings;

    public class BrakeSettingsMsg : Message
    {
        [ProtoField(1)] public float BrakeSpeed;
        [ProtoField(2)] public float BrakeDecel;
        [ProtoField(3)] public bool IsBlockSection;
        [ProtoField(4)] public BrakeOverflowSettingsMsg OverrideBrakeOverflowSettings;
    }

    [ProtoField(6)] public BrakeSettingsMsg BrakeSettings;

    public class LaunchSettingsMsg : Message
    {
        [ProtoField(1)] public float LaunchSpeed;
        [ProtoField(2)] public float LaunchAccel;
        [ProtoField(3)] public float BrakeSpeed;
        [ProtoField(4)] public float BrakeDecel;
        [ProtoField(5)] public LaunchOverflowSettingsMsg OverrideLaunchOverflowSettings;

        public enum LaunchMode
        {
            Standard = 0,
            BlockSection = 1,
            SwingForward = 2,
            SwingBackward = 3
        }

        [ProtoField(6)] public LaunchMode LaunchModeValue;
    }

    [ProtoField(7)] public LaunchSettingsMsg LaunchSettings;

    [ProtoField(9)] public string Name;
    [ProtoField(10)] public CoasterSegmentColorSchemeSettingsMsg OverrideColorSettings;
    [ProtoField(11)] public TrackVisualSettingsMsg OverrideTrackVisualSettings;
    [ProtoField(12)] public SupportGenerationSettingsMsg OverrideSupportGenerationSettings;
    [ProtoField(13)] public ExtrasGenerationSettingsMsg OverrideExtraGenerationSettings;
    [ProtoField(14)] public StructureGenerationSettingsMsg OverrideStructureGenerationSettings;
}