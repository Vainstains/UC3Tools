using VProto;

namespace LibUC3.Schema;

public class BankNodeSettingsMsg : Message
{
    [ProtoField(1)] public float ControlPointIndex;
    [ProtoField(2)] public float Angle;
    [ProtoField(3)] public bool Relative;
    [ProtoField(4)] public bool ContinuousRoll;
    [ProtoField(5)] public bool DisableRolls;
    [ProtoField(6)] public float Smoothness;
    [ProtoField(7)] public float HeartlinePercentage;
}