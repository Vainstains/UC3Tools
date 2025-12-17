using VProto;

namespace LibUC3.Schema;

public class TrainCarColorSchemeSettingsMsg : Message
{
    [ProtoField(1)] public ColorMsg ShellColor;
    [ProtoField(2)] public ColorMsg SecondaryShellColor;
    [ProtoField(3)] public ColorMsg BaseColor;
    [ProtoField(4)] public ColorMsg WingColor;
    [ProtoField(5)] public ColorMsg SeatColor;
    [ProtoField(6)] public ColorMsg RestraintColor;
    [ProtoField(7)] public ColorMsg WheelColor;
    [ProtoField(8)] public ColorMsg ChassisColor;
    [ProtoField(9)] public ColorMsg SecondaryWingColor;
    [ProtoField(10)] public ColorMsg SecondaryRestraintColor;
}