using VProto;

namespace LibUC3.Schema;

public class CoasterSettingsMsg : Message
{
    public enum TrainColorSchemeTypeEnum
    {
        AllSame,
        PerTrain,
        PerCar
    }
    
    [ProtoField(1)] public List<CoasterPartGroupSettingsMsg> PartGroups;
    [ProtoField(2)] public string TypeId;
    [ProtoField(3)] public int NumCarsWithSeatsInTrain;
    [ProtoField(4)] public int MaxNumTrains;
    [ProtoField(5)] public float Gravity;
    [ProtoField(6)] public float FrictionHalfLife;
    [ProtoField(7)] public string Name;
    [ProtoField(8)] public CoasterSegmentColorSchemeSettingsMsg DefaultSegmentColorScheme;
    [ProtoField(9)] public TrainColorSchemeTypeEnum TrainColorSchemeType;
    [ProtoField(10)] public List<TrainCarColorSchemeSettingsMsg> TrainCarColorSchemes;
    [ProtoField(11)] public string TrainTypeName;
    [ProtoField(12)] public List<CoasterTriggerSettingsMsg> Triggers;
    [ProtoField(13)] public SupportGenerationSettingsMsg OverrideSupportGenerationSettings;
    [ProtoField(14)] public float CameraShakeScale;
    [ProtoField(15)] public CoasterSoundSettingsMsg OverrideSoundSettings;
}